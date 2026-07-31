using System.Collections;
using System.Collections.Generic;
using Study.MiniDefence._3._Combat;
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
        private List<Enemy> inRange = new();    // 사거리 안 적들을 담아 두는 통

        // 주변 보너스
        private float bonusAtkPercent;
        private float bonusSpeedPercent;

        // Launcher
        public ProjectileLauncher Launcher { get; private set; }

        private IAttackPattern attackPattern = new SingleAttack();
        private ITargetingStrategy targetingStrategy;

        [SerializeField] private float attackWindUp;
        
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
            Launcher = GetComponent<ProjectileLauncher>(); //없을 수도 있다(Unit_02)
        }

        // 생성 직후 배치 시스템이 불러주는 초기화.
        public void Init(UnitDef def, Vector2Int coord)
        {
            Def = def;
            Coord = coord;

            attackPattern = AttackPatternFactory.Create(def.Kind);
            
            // 하이어라키에서 어떤 유닛이 어디에 있는지 바로 보이도록.
            gameObject.name = $"Unit_{def.Name}_{coord.x}_{coord.y}";

            StartCoroutine(AttackRoutine());
        }

        private void Update()
        {
            RefreshBuff();
        }

        #region 공격

        private IEnumerator AttackRoutine()
        {
            while (gameObject.activeInHierarchy)
            {
                Enemy target = AcquireTarget();
                
                if (target == null)
                {
                    yield return null; // 사거리에 아무도 없으면 다음프레임으로 넘긴다
                    continue;
                }
               
                // 사거리 안에 적이 있으면 공격 자세(Attack), 없으면 대기(Idle).
                animator.SetBool(IS_ATTACK, true);
                // 선 딜레이
                yield return new WaitForSeconds(attackWindUp);
                Attack(target);
                animator.SetBool(IS_ATTACK, false);
                // 후 딜레이 (전체 딜레이 - 선딜레이)
                yield return new WaitForSeconds(AttackInterval - attackWindUp);
                // 쿨다운이 끝나야 다음 공격을 진행한다
            }
        }
        
        // 사거리 안에서 조준 기준에 맞는 적 하나를 고른다.
        private Enemy AcquireTarget()
        {
            // inRange에 조건을 만족하는 Enemy 객체가 채워져서 나온다
            EnemyRegistry.FindInRange(transform.position, Def.Range, ref inRange);
            if(inRange.Count == 0)return null;
            return inRange[0]; // 일단은 요걸로
        }

        private void Attack(Enemy target)
        {
            // 크리티컬 판정은 한발에 한번만
            // Random.value ?
            // : 0~1 까지의 float중 하나를 반환합니다. 0~100 까지 사용했지만, 이제부터는
            //  섞어서 쓰겠습니다.
            bool isCritical = Random.value < Def.CritChance;
            int damage = isCritical
                ? Mathf.RoundToInt(Def.Damage * GameSettings.Instance.CriMultiplier)
                : Def.Damage;
            
            attackPattern.Fire(this,target,damage, isCritical);
        }

        #endregion

        #region 지휘관 강화

        private void RefreshBuff()
        {
            
        }

        #endregion
    }
}