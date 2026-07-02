using System.Collections;
using UnityEngine;
using static Geometry.EnemyUnit;

namespace Geometry
{
    public class MeleeUnit : PlayerUnit
    {

        private void Update()
        {
            if (current_Hp <= 0)
            {
                Debug.Log($"{this.name} 사망");
                PlayerManager.Instance.Dead(this);
            }
        }

        private void OnEnable()
        {
            StartCoroutine(FiniteStateMachineCoroutine());
        }

        private IEnumerator FiniteStateMachineCoroutine()
        {
            nextStateCoroutine = ScanCoroutine();

            while (gameObject.activeInHierarchy)
            {
                yield return StartCoroutine(nextStateCoroutine);
            }
        }


        private IEnumerator ScanCoroutine()
        {
            while (true)
            {
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, targetLayer);

                if (hitEnemies.Length > 0)
                {
                    targetEnemy = GetClosestEnemy(hitEnemies);
                    if(targetEnemy != null)
                    {
                        nextStateCoroutine = AttackCoroutine();
                        yield break;
                    }
                }
                targetEnemy = null;
                yield return new WaitForSeconds(scanCooldown);
            }
        }


        private IEnumerator AttackCoroutine()
        {
            while (true)
            {
                if (targetEnemy == null||targetEnemy.current_Hp<=0)
                {
                    nextStateCoroutine = ScanCoroutine();
                    yield break;
                }

                Collider2D targetCol = targetEnemy.GetComponent<Collider2D>();

                if (targetCol !=null)
                {
                    Vector3 closestEdge = targetCol.ClosestPoint(transform.position);
                    float distanceToEdge = Vector3.Distance(transform.position, closestEdge);

                    if (distanceToEdge > attackRange)
                    {
                        nextStateCoroutine = ScanCoroutine();
                        yield break;
                    }
                }
                else
                {
                    nextStateCoroutine = ScanCoroutine();
                    yield break;
                }
                yield return new WaitForSeconds(attackDelay);
                Debug.Log($"{this.name}이 {targetEnemy}에게 {attackPower}만큼 공격");
                targetEnemy.TakeDamage(attackPower, myType);
                yield return new WaitForSeconds(attackCooldown - attackDelay);
            }
        }

        private EnemyUnit GetClosestEnemy(Collider2D[] enemies)
        {
            Transform closestEnemy = null; // 가까운 적의 위치를 담기위해
            float closestDistance = Mathf.Infinity; //거리 비교를 하기 위해 가장 큰 수
            Vector3 currentPosition = transform.position; // 내 위치를 알기 위해

            foreach (Collider2D enemy in enemies) //위에서 담아온 적들을 collider2D enemy로 하나씩 넣기
            {
                EnemyUnit enemyComponent = enemy.GetComponent<EnemyUnit>();

                if(enemyComponent != null && enemyComponent.current_Hp > 0)
                {
                    Vector3 closet = enemy.ClosestPoint(currentPosition);
                    float distanceTo = Vector3.Distance(currentPosition, closet);

                    if(distanceTo< closestDistance)
                    {
                        closestDistance = distanceTo;
                        closestEnemy = enemy.transform;
                    }
                }
            }
            if (closestEnemy != null)
            {
                return closestEnemy.GetComponent<EnemyUnit>();
            }
            return null;
        }

        public override void TakeDamage(float damage, EnemyUnit.EnemyType type)
        {
            float addDamage = 1.0f;
            if (myType == PlayerManager.UnitType.Circle)
            {
                // 네모, 세모만
                if (type == EnemyType.Square)
                {
                    addDamage = 0.5f;
                }
                else if (type == EnemyType.Triangle)
                {
                    addDamage = 2.0f;
                }
            }
            else if (myType == PlayerManager.UnitType.Square)
            {
                // 동그라미, 세모만
                if (type == EnemyType.Triangle)
                {
                    addDamage = 2.0f;
                }
                else if (type == EnemyType.Circle)
                {
                    addDamage = 0.5f;
                }
            }
            //Debug.Log($"{this.name}이 {type}에게 {damage * addDamage}만큼 공격 받음");
            current_Hp -= damage * addDamage;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}