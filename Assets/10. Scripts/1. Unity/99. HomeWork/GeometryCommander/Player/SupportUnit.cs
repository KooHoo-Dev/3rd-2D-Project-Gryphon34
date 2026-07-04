using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Geometry
{
    public class SupportUnit : PlayerUnit
    {
        [SerializeField] private float duration = 3f;

        protected override void Awake()
        {

        }
        private void OnEnable()
        {
            StartCoroutine(AttackCoroutine());
        }

        private IEnumerator AttackCoroutine()
        { 
            HashSet<EnemyUnit> alreadyDamaged = new HashSet<EnemyUnit>();
            float timer = 0f;
            while (duration>=timer)
            {
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, targetLayer);

                foreach (Collider2D collider in hitEnemies)
                {
                   EnemyUnit enemy = collider.gameObject.GetComponent<EnemyUnit>();
                    if(enemy !=null && alreadyDamaged.Contains(enemy) == false)
                    {
                        enemy.TakeDamage(attackPower, myType);
                        Debug.Log($"SupportUnit이 {enemy.name}을 공격");
                        alreadyDamaged.Add(enemy);
                    }
                }
                timer += Time.deltaTime;
                yield return null;
            }
        }

        protected override void DeadState()
        {
            
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}