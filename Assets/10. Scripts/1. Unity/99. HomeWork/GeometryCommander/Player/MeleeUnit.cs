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
                    nextStateCoroutine = AttackCoroutine();
                    yield break;
                }
                else
                {
                    targetEnemy = null;
                }
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

                float distanceToTarget = Vector3.Distance(targetEnemy.transform.position, this.transform.position);

                if (distanceToTarget >= attackRange)
                {
                    nextStateCoroutine = ScanCoroutine();
                    yield break;
                }
                Debug.Log($"공격 : {targetEnemy.name}, {distanceToTarget}");
                targetEnemy.TakeDamage(attackPower, myType);
                yield return new WaitForSeconds(attackCooldown);
            }
        }

        private EnemyUnit GetClosestEnemy(Collider2D[] enemies)
        {
            Transform closestEnemy = null; // 가까운 적의 위치를 담기위해
            float closestDistanceSqr = Mathf.Infinity; //거리 비교를 하기 위해 가장 큰 수
            Vector3 currentPosition = transform.position; // 내 위치를 알기 위해

            foreach (Collider2D enemy in enemies) //위에서 담아온 적들을 collider2D enemy로 하나씩 넣기
            {
                Vector3 directionalToTarget = enemy.transform.position - currentPosition; // 거리 계산 적 위치 - 내 위치

                float dSqrToTarget = directionalToTarget.sqrMagnitude; // 거리의 제곱

                if (dSqrToTarget < closestDistanceSqr) // 거리의 제곱보다 가까운 적의 위치가 멀다면
                {
                    closestDistanceSqr = dSqrToTarget; // 가까운 적의 위치를 방금 구한 거리의 적을 삽입
                    closestEnemy = enemy.transform; // 가까운 적의 위치= 에너미의 위치
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}