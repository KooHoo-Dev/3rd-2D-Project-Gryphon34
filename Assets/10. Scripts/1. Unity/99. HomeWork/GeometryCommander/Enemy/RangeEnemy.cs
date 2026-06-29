using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Geometry
{
    public class RangeEnemy : EnemyUnit
    {
        private void Update()
        {
            if (current_Hp <= 0 && !IsDead)
            {
                IsDead = true;
                Debug.Log($"{enemyType} 사망");
                PlayerManager.Instance.AddCost(reward);

                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            StartCoroutine(FiniteStateMachineCoroutine());
        }

        private IEnumerator FiniteStateMachineCoroutine()
        {
            nextStateCoroutine = ForwardCoroutine();

            while (gameObject.activeInHierarchy)
            {
                yield return StartCoroutine(nextStateCoroutine);
            }
        }

        private IEnumerator ForwardCoroutine()
        {
            while (true)
            {
                Vector3 enemyPos = transform.position;
                Vector3 basePosition = playerBase.transform.position;
                CheckPathBlock();
                if (IsBlock || IsBase)
                {
                    nextStateCoroutine = AttackCoroutine();
                    yield break;
                }
                Vector3 dir = (basePosition - enemyPos).normalized;
                transform.up = dir;
                transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
                yield return null;
            }
        }

        private void CheckPathBlock()
        {
            if (IsBlock == true || IsBase == true)
            {
                return;
            }
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2.0f);

            foreach (Collider2D hit in hits)
            {
                if (hit.GetComponent<EnemyUnit>())
                {
                    continue;
                }
                Vector3 dirToTarget = (hit.transform.position - transform.position).normalized;
                if (Vector3.Angle(transform.up, dirToTarget) < viewAngle / 2)
                {
                    if (hit.GetComponent<PlayerUnit>())
                    {
                        IsBlock = true;
                        targetUnit = hit.GetComponent<PlayerUnit>();
                        return;
                    }
                    else if (hit.GetComponent<Base>())
                    {
                        IsBase = true;
                        playerBase = hit.GetComponent<Base>();
                        return;
                    }
                }
            }
        }

        private IEnumerator AttackCoroutine()
        {
            while (true)
            {
                if (IsBlock)
                {
                    if (targetUnit == null || targetUnit.current_Hp<=0)
                    {
                        IsBlock = false;
                        nextStateCoroutine = ForwardCoroutine();
                        yield break;
                    }

                    if (Vector3.Distance(targetUnit.transform.position, transform.position) <= attackRange)
                    {
                        Debug.Log($"{this.name}이 {targetUnit}에게 {attackPower}만큼 공격");
                        targetUnit.TakeDamage(attackPower, enemyType);
                    }

                    yield return new WaitForSeconds(attackCooldown);
                }
                else if (IsBase)
                {
                    if (playerBase == null)
                    {
                        IsBase = false;
                        nextStateCoroutine = ForwardCoroutine();
                        yield break;
                    }
                    playerBase.BaseTakeDamage(attackPower);
                    yield return new WaitForSeconds(attackCooldown);
                }
                else
                {
                    nextStateCoroutine = ForwardCoroutine();
                    yield break;
                }
            }
        }

        public override void TakeDamage(float damage, PlayerManager.UnitType type)
        {
            float addDamage = 1.0f;
            if (type == PlayerManager.UnitType.Circle)
            {
                addDamage = 0.5f;
            }
            else if (type == PlayerManager.UnitType.Square)
            {
                addDamage = 2.0f;
            }
            Debug.Log($"{enemyType}이 {type}에게 {damage * addDamage}만큼 공격 받음");
            current_Hp -= damage * addDamage;
        }
    }
}