using System.Collections;
using UnityEngine;

namespace Geometry
{
    public class RangeEnemy : EnemyUnit
    {
        private void Update()
        {
            if (current_Hp <= 0)
            {
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
                    return;
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
                if (IsBlock||IsBase)
                {
                    if (targetUnit == null)
                    {
                        nextStateCoroutine = ForwardCoroutine();
                        yield break;
                    }

                    yield return new WaitForSeconds(attackDelay);
                    targetUnit.TakeDamage(attackPower, enemyType);
                    yield return new WaitForSeconds(attackCooldown - attackDelay);
                }
                else if (IsBase)
                {
                    if (playerBase == null)
                    {
                        nextStateCoroutine = ForwardCoroutine();
                        yield break;
                    }

                    yield return new WaitForSeconds(attackDelay);
                    targetUnit.TakeDamage(attackPower, enemyType);
                    yield return new WaitForSeconds(attackCooldown - attackDelay);
                }
            }
        }

        public override void TakeDamage(float damage, PlayerUnit.UnitType type)
        {
            float addDamage = 1.0f;
            if (type == PlayerUnit.UnitType.Circle)
            {
                addDamage = 0.5f;
            }
            else if (type == PlayerUnit.UnitType.Square)
            {
                addDamage = 2.0f;
            }
            Debug.Log($"{enemyType}이 {type}에게 {damage * addDamage}만큼 공격 받음");
            current_Hp -= damage * addDamage;
        }
    }
}