using UnityEngine;

namespace Geometry
{
    public class RangeEnemy : EnemyUnit
    {
        public override void TakeDamage(float damage, PlayerUnit.UnitType type)
        {
            float addDamage = 1.0f;
            if (type == PlayerUnit.UnitType.Circle)
            {
                addDamage = 0.5f;
            }
            else if (type == PlayerUnit.UnitType.Square)
            {
                addDamage = 2.0f;
            }
            maxHp -= damage * addDamage;
        }
    }
}