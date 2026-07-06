using System.Collections;
using UnityEngine;

namespace Geometry
{
    public class RangeEnemy : EnemyUnit
    {

        [SerializeField] private GameObject bulletPrefab;
        private void Update()
        {
            if (current_Hp <= 0 && !IsDead)
            {
                IsDead = true;
                PlayerManager.Instance.AddCost(reward);
                DeadState();
            }
            updateHpBar();
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
                transform.position = (transform.position + dir * moveSpeed * Time.deltaTime);
                yield return null;
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

                    Collider2D targetCol = targetUnit.GetComponent<Collider2D>();

                    if (targetCol != null)
                    {
                        Vector3 closestEdge = targetCol.ClosestPoint(transform.position);
                        float distanceToEdge = Vector3.Distance(transform.position, closestEdge);

                        if (distanceToEdge > attackRange)
                        {
                            nextStateCoroutine = ForwardCoroutine();
                            yield break;
                        }
                    }
                    else
                    {
                        nextStateCoroutine = ForwardCoroutine();
                        yield break;
                    }
                    GameObject bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

                    Bullet bullet = bulletObj.GetComponent<Bullet>();

                    float dir = targetUnit.transform.position.x - transform.position.x;

                    Vector3 left = Vector3.right * Mathf.Sign(dir);
                    bullet.SetTarget(targetUnit.gameObject, left);
                    yield return new WaitForSeconds(attackDelay);
                    Debug.Log($"{this.name}이 {targetUnit}에게 {attackPower}만큼 공격");
                    targetUnit.TakeDamage(attackPower, enemyType);
                    yield return new WaitForSeconds(attackCooldown- attackDelay);
                }
                else if (IsBase)
                {
                    if (playerBase == null)
                    {
                        IsBase = false;
                        nextStateCoroutine = ForwardCoroutine();
                        yield break;
                    }
                    yield return new WaitForSeconds(attackDelay);
                    Debug.Log($"{this.name}이 {playerBase}에게 {attackPower}만큼 공격");
                    playerBase.BaseTakeDamage(attackPower);
                    yield return new WaitForSeconds(attackCooldown - attackDelay);
                }
                else
                {
                    nextStateCoroutine = ForwardCoroutine();
                    yield break;
                }
            }
        }

        public override void TakeDamage(float damage, PlayerManager.UnitType type, PlayerUnit attacker)
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
            current_Hp -= damage * addDamage;
            if (current_Hp <= 0 && !IsDead) attacker.current_Morale += 10.0f;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}