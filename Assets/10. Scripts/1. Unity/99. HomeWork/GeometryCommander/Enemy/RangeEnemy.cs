
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
                Debug.Log($"{enemyType} 사망");
                PlayerManager.Instance.AddCost(reward);

                Destroy(gameObject);
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
                Vector3 basePos = playerBase.transform.position;
                CheckPathBlock();
                if (IsBlock || IsBase)
                {
                    nextStateCoroutine = AttackCoroutine();
                    yield break;
                }
                Vector3 dir = (basePos - enemyPos).normalized;
                transform.position = (transform.position + dir * moveSpeed * Time.deltaTime);
                yield return null;
            }
        }

        private void CheckPathBlock()
        {
            if (IsBlock == true || IsBase == true)
            {
                return;
            }
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);

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
            current_Hp -= damage * addDamage;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}