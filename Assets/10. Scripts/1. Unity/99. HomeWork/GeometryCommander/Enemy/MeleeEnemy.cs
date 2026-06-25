using UnityEngine;
using static Geometry.EnemyUnit;

namespace Geometry
{
    public class MeleeEnemy : EnemyUnit
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void TakeDamage(float damage, PlayerUnit.UnitType type)
        {
            float addDamage = 1.0f;
            if (enemyType == EnemyType.Circle)
            {
                // 네모, 세모만
                if (type == PlayerUnit.UnitType.Square)
                {
                    addDamage = 0.5f;
                }
                else if (type == PlayerUnit.UnitType.Triangle)
                {
                    addDamage = 2.0f;
                }
            }
            else if (enemyType == EnemyType.Square)
            {
                // 동그라미, 세모만
                if (type == PlayerUnit.UnitType.Triangle)
                {
                    addDamage = 2.0f;
                }
                else if (type == PlayerUnit.UnitType.Circle)
                {
                    addDamage = 0.5f;
                }
            }
            maxHp-=damage*addDamage;
        }
    }
}

