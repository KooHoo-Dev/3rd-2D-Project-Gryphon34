using UnityEngine;
using UnityEngine.InputSystem;
using static Geometry.EnemyUnit;

namespace Geometry
{
    public class RangeUnit : PlayerUnit
    {
        [Header("유닛 정보")]
        [SerializeField] private float attackRange = 2.0f; // 이 유닛 고유의 사거리 (기본값 5)
        [SerializeField] private LayerMask targetLayer;

        
        protected override void Attack()
        {

        }

        protected override void FindTarget()
        {

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
            maxHp -= damage * addDamage;
        }
    }
}

