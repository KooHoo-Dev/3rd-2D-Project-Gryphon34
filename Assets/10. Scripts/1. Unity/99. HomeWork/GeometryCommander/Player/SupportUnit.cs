using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Geometry
{
    public class SupportUnit : PlayerUnit
    {
        [SerializeField] private float duration = 3f;
        [SerializeField] private GameObject bombPrefab;
        [SerializeField] private float dropInterval = 1f;

        protected override void Awake()
        {
           
        }

        protected override void Start()
        {
            base.Start();
            isDead = false;
        }

        protected override void Update()
        {
            if (isDead)
            {
                StopAllCoroutines();
                DeadState();
            }
        }
        private void OnEnable()
        {
            StartCoroutine(AttackCoroutine());
        }

        private IEnumerator AttackCoroutine()
        { 
            HashSet<EnemyUnit> alreadyDamaged = new HashSet<EnemyUnit>();
            float timer = 0f;
            float dropTimer = 0f;
            while (duration>=timer)
            {
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, targetLayer);

                if (dropTimer <= 0f && bombPrefab != null)
                {
                    Vector2 randomPos = Random.insideUnitCircle * attackRange;
                    Vector3 targetPos = transform.position + new Vector3(randomPos.x, randomPos.y, 0f);

                    Instantiate(bombPrefab, targetPos, Quaternion.Euler(0, 0, -90f));
                    dropTimer = dropInterval;
                }

                foreach (Collider2D collider in hitEnemies)
                {
                   EnemyUnit enemy = collider.gameObject.GetComponent<EnemyUnit>();
                    if(enemy !=null && alreadyDamaged.Contains(enemy) == false)
                    {
                        enemy.TakeDamage(attackPower, myType,this);
                        Debug.Log($"SupportUnit이 {enemy.name}을 공격");
                        alreadyDamaged.Add(enemy);
                    }
                }
                
                timer += Time.deltaTime;
                dropTimer-= Time.deltaTime;
                yield return null;
            }
            isDead = true;
            yield break;
        }

        protected override void DeadState()
        {
            deadState.SetActive(true);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}