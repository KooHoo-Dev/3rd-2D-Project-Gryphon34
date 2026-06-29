using UnityEngine;
using System.Collections;

namespace Geometry
{
    public abstract class PlayerUnit : MonoBehaviour
    {
        // enum : 유닛_타입(동그라미_근접, 네모_근접, 세모_원거리)
        // float 변수: 최대_체력, 현재_체력, 최대_사기, 현재_사기, 기본_공격력, 사거리
        // bool 변수 : 최초_조우_상태
        // GameObject : 현재_타겟

        

        [SerializeField] protected float attackRange;
        [SerializeField] protected float scanCooldown;
        [SerializeField] protected float attackCooldown;
        [SerializeField] protected float attackPower;
        [SerializeField] protected float maxHp;
        [SerializeField] protected float maxMorale;
        [SerializeField] protected LayerMask targetLayer;

        protected IEnumerator nextStateCoroutine;
        public PlayerManager.UnitType myType;
        public EnemyUnit targetEnemy;
        public float current_Hp;
        public float current_Morale;


        public  virtual void TakeDamage(float damage, EnemyUnit.EnemyType type)
        {

        }
    }

}
