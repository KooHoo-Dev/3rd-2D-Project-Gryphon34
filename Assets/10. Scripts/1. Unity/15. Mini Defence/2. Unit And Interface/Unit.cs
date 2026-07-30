using System.Collections.Generic;
using UnityEngine;

namespace Study.MiniDefence
{
    // 타일 위에 배치된 아군 유닛 개체.
    //
    // 스탯을 스스로 들고 있지 않다는 점이 중요하다.
    // "나는 어떤 종류(UnitDef)인가"만 기억하고, 실제 수치는 그때그때 데이터에서 꺼내 쓴다.
    //   → 표(units.tsv)만 고치면 게임 안의 모든 유닛이 함께 바뀐다.
    public class Unit : MonoBehaviour
    {
        private static readonly int IS_ATTACK = Animator.StringToHash("IsAttack");

        public UnitDef Def { get; private set; }
        public Vector2Int Coord { get; private set; }   // 내가 점유한 타일 좌표

        private Animator animator;

        private float cooldown;                          // 다음 공격까지 남은 시간
        private readonly List<Enemy> inRange = new();    // 사거리 안 적들을 담아 두는 통

        // 주변 보너스
        private float bonusAtkPercent;
        private float bonusSpeedPercent;

        #region 스탯 (기본값 + 강화)

        public int Damage => Mathf.RoundToInt(Def.Damage * (1f + bonusAtkPercent));
        public float Range => Def.Range;
        public float CritChance => Def.CritChance;

        // 공격 속도가 빨라진다 = 공격 간격이 짧아진다.
        public float AttackInterval => Def.AttackInterval / (1f + bonusSpeedPercent);

        #endregion

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        // 생성 직후 배치 시스템이 불러주는 초기화.
        public void Init(UnitDef def, Vector2Int coord)
        {
            Def = def;
            Coord = coord;

            // 하이어라키에서 어떤 유닛이 어디에 있는지 바로 보이도록.
            gameObject.name = $"Unit_{def.Name}_{coord.x}_{coord.y}";
        }

        private void Update()
        {
            RefreshBuff();

            Enemy target = AcquireTarget();

            // 사거리 안에 적이 있으면 공격 자세(Attack), 없으면 대기(Idle).
            animator.SetBool(IS_ATTACK, target != null);

            // 쿨다운이 끝나야 다음 발을 쏜다.
            cooldown -= Time.deltaTime;
            if (cooldown > 0f) return;
            if (target == null) return;   // 사거리에 아무도 없으면 그냥 기다린다

            Attack(target);
            cooldown = AttackInterval;
        }

        #region 공격

        // 사거리 안에서 조준 기준에 맞는 적 하나를 고른다.
        private Enemy AcquireTarget()
        {
            return null;
        }

        private void Attack(Enemy target)
        {
            
        }

        #endregion

        #region 지휘관 강화

        private void RefreshBuff()
        {
            
        }

        #endregion
    }
}