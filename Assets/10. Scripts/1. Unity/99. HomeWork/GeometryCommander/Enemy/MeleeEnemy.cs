using UnityEngine;
using System.Collections;

namespace Geometry
{
    public class MeleeEnemy : EnemyUnit
    {
        public SpriteRenderer attackEffect;
        private Color alphaValue;
        protected override void Awake()
        {
            base.Awake();
            alphaValue = attackEffect.color;
            alphaValue.a = 0f;
            attackEffect.color = alphaValue;
        }

        private void Update()
        {
            if (current_Hp <= 0 && !IsDead)
            {
                IsDead = true;
                StopAllCoroutines();
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
                    nextStateCoroutine=AttackCoroutine();
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
                    if (targetUnit == null||targetUnit.current_Hp<=0)
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
                    alphaValue.a = 1f;
                    attackEffect.color = alphaValue;
                    yield return new WaitForSeconds(attackDelay);
                    alphaValue.a = 0f;
                    Debug.Log($"{this.name}이 {targetUnit}에게 {attackPower}만큼 공격");
                    targetUnit.TakeDamage(attackPower, enemyType);
                    attackEffect.color = alphaValue;
                    yield return new WaitForSeconds(attackCooldown - attackDelay);
                }
                else if (IsBase)
                {
                    if (playerBase == null)
                    {
                        IsBase = false;
                        nextStateCoroutine = ForwardCoroutine();
                        yield break;
                    }
                    alphaValue.a = 1f;
                    attackEffect.color = alphaValue;
                    yield return new WaitForSeconds(attackDelay);
                    
                    alphaValue.a = 0f;
                    Debug.Log($"{this.name}이 {playerBase}에게 {attackPower}만큼 공격");
                    playerBase.BaseTakeDamage(attackPower);
                    attackEffect.color = alphaValue;
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
            if (enemyType == EnemyType.Circle)
            {
                // 네모, 세모만
                if (type == PlayerManager.UnitType.Square)
                {
                    addDamage = 0.5f;
                }
                else if (type == PlayerManager.UnitType.Triangle)
                {
                    addDamage = 2.0f;
                }
            }
            else if (enemyType == EnemyType.Square)
            {
                // 동그라미, 세모만
                if (type == PlayerManager.UnitType.Triangle)
                {
                    addDamage = 2.0f;
                }
                else if (type == PlayerManager.UnitType.Circle)
                {
                    addDamage = 0.5f;
                }
            }
            Debug.Log($"{enemyType}이 {type}에게 {damage*addDamage}만큼 공격 받음");
            current_Hp -=damage*addDamage;

            if(current_Hp<=0 && !IsDead)
            {
                attacker.current_Morale += 10.0f;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}

