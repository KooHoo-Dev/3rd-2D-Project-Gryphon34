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
                    targetEnemy = GetLowHealthEnemy(hitEnemies);
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

        private EnemyUnit GetLowHealthEnemy(Collider2D[] enemies)
        {
            EnemyUnit currentTargetEnemy = null;
            float currentLowestHealth = Mathf.Infinity;

            foreach(Collider2D enemy in enemies)
            {
                EnemyUnit enemyComponent = enemy.GetComponent<EnemyUnit>();

                if(enemyComponent != null && enemyComponent.current_Hp > 0)
                {
                    float checkingHealth = enemyComponent.current_Hp;
                    if (currentLowestHealth >= checkingHealth)
                    {
                        currentLowestHealth = checkingHealth;
                        currentTargetEnemy = enemyComponent;
                    }
                }
            }
            return currentTargetEnemy;
        }

        private IEnumerator AttackCoroutine()
        {
            while (true)
            {
                if (targetEnemy == null || targetEnemy.current_Hp <= 0)
                {
                    nextStateCoroutine = ScanCoroutine();
                    yield break;
                }

                if (Vector3.Distance(targetEnemy.transform.position, this.transform.position) >= attackRange)
                {
                    nextStateCoroutine = ScanCoroutine();
                    yield break;
                }
                Debug.Log($"{this.name}이 {targetEnemy}에게 {attackPower}만큼 공격");
                targetEnemy.TakeDamage(attackPower, myType);
                yield return new WaitForSeconds(attackCooldown);
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
            current_Hp -= damage * addDamage;
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}

