using Geometry;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static Geometry.EnemyUnit;

namespace Geometry
{
    public class MeleeUnit : PlayerUnit
    {
        /*
         * 일단 지금 해볼건 그냥 사거리 안에 들어오면 감지하는걸 해보자.
         * Physics2D.OverShpere를 해서 콜라이더를 갖고 있는 객체 모두를 배열에 담아.
         * 거기에서 가장 가까운 객체만 뽑아.
         */

        [Header("유닛 정보")]
        [SerializeField] private float attackRange = 1.3f; // 이 유닛 고유의 사거리 (기본값 5)
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private float scanInterval = 0.2f;

        private EnemyUnit targetEnemy;

        private void Start()
        {
            StartCoroutine(ScanCoroutine());
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

        protected override void Attack()
        {
            if (targetEnemy != null)
            {

            }
        }

        private IEnumerator ScanCoroutine()
        {
            while (true)
            {
                FindTarget();
                yield return new WaitForSeconds(scanInterval);
            }
        }

        protected override void FindTarget()
        {

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, targetLayer);

            if (hitEnemies.Length > 0)
            {
                targetEnemy = GetClosestEnemy(hitEnemies);

                Debug.Log($"[{gameObject.name}] 타겟 지정 완료: {targetEnemy.name}");
            }
            else
            {
                targetEnemy = null;
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
            if (myType == UnitType.Circle)
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
            else if (myType == UnitType.Square)
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
            maxHp -= damage * addDamage;
        }
    }
}