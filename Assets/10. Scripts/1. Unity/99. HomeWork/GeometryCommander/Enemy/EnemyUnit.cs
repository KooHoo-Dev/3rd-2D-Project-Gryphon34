using UnityEngine;

namespace Geometry
{
    public class EnemyUnit : MonoBehaviour
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

        [SerializeField]
        public EnemyType enemyType;
        [SerializeField]
        public float maxHp;
        [SerializeField]
        private float attackPower;
        [SerializeField]
        private float attackRange;
        [SerializeField]
        private float moveSpeed;

        protected bool IsBlock;
        public float current_Hp;

        public virtual void TakeDamage(float damage, PlayerUnit.UnitType type)
        {

        }
    }
    // 동그라미는 세모한테 약하고 세모는 네모한테 약하고 네모는 동그라미한테 약하고
    // 경우의 수 
    // 세모(0): 동그라미(1) 세모가 이김
    // 세모(0) : 네모(2) 세모가 짐
    // 동그라미(1) : 세모(0) 동그라미가 짐
    // 동그라미(1) : 네모(2) 동그라미가 이김
    // 네모(2) : 세모(0) 네모가 이김
    // 네모(2) : 동그라미(1) 네모가 짐

    // 데미지를 입는다
    // 플레이어의 유닛의 사거리 안에 들어와서 타겟에 삽입됐어
    // 플레이어가 본인의 데미지와 타입을 건네 줘
    // TakeDamage에
}