using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static Geometry.EnemyUnit;

namespace Geometry
{
    public class RangeUnit : PlayerUnit
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
                    targetEnemy = GetLowHealthEnemy(hitEnemies);
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

        private EnemyUnit GetLowHealthEnemy(Collider2D[] enemies)
        {
            EnemyUnit currentTargetEnemy = enemies[0].GetComponent<EnemyUnit>(); // 기준 적
            float currentLowestHealth = currentTargetEnemy.current_Hp; // 기준 적의 체력

            foreach(Collider2D enemy in enemies)
            {
                float checkingHealth = enemy.GetComponent<EnemyUnit>().current_Hp; // 지금 확인 할 적의 체력
                if(currentLowestHealth >= checkingHealth) // 만약 기준 적의 체력이 지금 확인 한 적의 체력보다 높으면
                {
                    currentLowestHealth = checkingHealth;
                    currentTargetEnemy = enemy.GetComponent<EnemyUnit>();
                }
            }

            if (currentTargetEnemy != null)
            {
                return currentTargetEnemy;
            }
            return null;
        }

        private IEnumerator AttackCoroutine()
        {
            while (true)
            {
                if (targetEnemy == null)
                {
                    nextStateCoroutine = ScanCoroutine();
                    yield break;
                }

                if (targetEnemy != null && Vector3.Distance(targetEnemy.transform.position, this.transform.position) >= attackRange)
                {
                    nextStateCoroutine = ScanCoroutine();
                    yield break;
                }

                yield return new WaitForSeconds(attackDelay);
                Debug.LogWarning("공격");
                targetEnemy.TakeDamage(attackPower, myType);
                yield return new WaitForSeconds(attackCooldown - attackDelay);
            }
        }

        public override void TakeDamage(float damage, EnemyUnit.EnemyType type)
        {
            float addDamage = 1.0f;
            if (type == EnemyType.Circle)
            {
                addDamage = 0.5f;
            }
            else if (type == EnemyType.Square)
            {
                addDamage = 2.0f;
            }
            Debug.Log($"{this.name}이 {type}에게 {damage * addDamage}만큼 공격 받음");
            current_Hp -= damage * addDamage;
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}

