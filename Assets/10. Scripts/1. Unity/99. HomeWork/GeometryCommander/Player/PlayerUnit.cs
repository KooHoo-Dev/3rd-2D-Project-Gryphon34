using UnityEngine;

namespace Geometry
{
    public abstract class PlayerUnit : MonoBehaviour
    {
        // enum : 유닛_타입(동그라미_근접, 네모_근접, 세모_원거리)
        // float 변수: 최대_체력, 현재_체력, 최대_사기, 현재_사기, 기본_공격력, 사거리
        // bool 변수 : 최초_조우_상태
        // GameObject : 현재_타겟
        // FSM
        // 여기에서의 TakeDamage는 내가 적에게 피해를 입는거임
        // 그러면 적의 Type과 내 타입을 알아야됨
        // 내 타입은 알수 있을거 같은데
        // 적의 타입은 어떻게 알까
        // 보낼때 

        public enum UnitType
        {
            Triangle,
            Circle,
            Square,
        }

        enum State
        {
            Idle,
            Move,
            Attack,
            Dead,
        }

        [SerializeField]
        public float maxHp;
        [SerializeField]
        private float maxMorale = 100f;
        [SerializeField]
        protected float attackPower;
        [SerializeField]
        protected UnitType myType;

        private State currentState = State.Idle;

        public float current_Hp;
        public float current_Morale;
        public bool First_Contact = true;

        protected abstract void FindTarget();

        protected abstract void Attack();

        public  virtual void TakeDamage(float damage, EnemyUnit.EnemyType type)
        {

        }
    }

}
