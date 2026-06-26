using UnityEngine;
using Study.Utilities;
using System.Collections;

namespace Geometry
{
    public abstract class EnemyUnit : MonoBehaviour
    {
        //enum : 적 타입(동그라미, 네모, 세모)
        // float 변수: 최대_체력, 현재_체력, 이동속도, 기본_공격력, 사거리
        // bool 변수: 길이_막혔는가
        public enum EnemyType
        {
            Triangle,
            Circle,
            Square,
        }

        [SerializeField] protected float attackRange;
        [SerializeField] protected float attackCooldown;
        [SerializeField] protected float attackDelay;
        [SerializeField] protected float attackPower;
        [SerializeField] protected float maxHp;
        [SerializeField] protected float moveSpeed;
        [SerializeField] protected float viewAngle = 100f;
        [SerializeField]protected Base playerBase;

        protected IEnumerator nextStateCoroutine;
        public EnemyType enemyType;
        public PlayerUnit targetUnit;
        protected bool IsBlock;
        protected bool IsBase;
        public float current_Hp;
        
        
        

        public virtual void TakeDamage(float damage, PlayerUnit.UnitType type)
        {

        }
    }
}