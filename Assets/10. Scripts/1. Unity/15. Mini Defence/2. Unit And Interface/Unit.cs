using System;
using System.Collections;
using System.Collections.Generic;
using Study.MiniDefence._3._Combat;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using Random = UnityEngine.Random;

namespace Study.MiniDefence
{
    // 타일 위에 배치된 아군 유닛 개체.
    //
    // 스탯을 스스로 들고 있지 않다는 점이 중요하다.
    // "나는 어떤 종류(UnitDef)인가"만 기억하고, 실제 수치는 그때그때 데이터에서 꺼내 쓴다.
    //   → 표(units.tsv)만 고치면 게임 안의 모든 유닛이 함께 바뀐다.
    public class Unit : MonoBehaviour
    {
        public Transform TargetPointer;
        public Vector3 TargetPointerOffset;
        
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

        private IAttackPattern attackPattern;
        private ITargetingStrategy targetingStrategy;

        [SerializeField] private float attackWindUp;
        
        #region 스탯 (기본값 + 강화)

        public UnitStats Stats { get; private set; }
        
        public int Damage => Stats.Damage;
        public float Range => Stats.Range;
        public float CritChance => Stats.CritChance;
        public float AttackInterval => Stats.AttackInterval;

        #endregion

        private void Awake()
        {
            animator = GetComponent<Animator>();
            Launcher = GetComponent<ProjectileLauncher>(); //없을 수도 있다(Unit_02)
        }

        private void OnEnable()
        {
            // Preview 개체일때 null 에러 뜸
            UnitRegistry.Register(this);
        }

        private void OnDisable()
        {
            UnitRegistry.Unregister(this);
        }

        // 생성 직후 배치 시스템이 불러주는 초기화.
        public void Init(UnitDef def, Vector2Int coord)
        {
            Def = def;
            Coord = coord;
            Stats = new UnitStats(Def);

            attackPattern = AttackPatternFactory.Create(def.Kind);
            targetingStrategy = TargetingFactory.Create(def.Targeting);
            
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

            Enemy target = targetingStrategy.Pick(inRange, transform.position);
            if (TargetPointer == null) return target;
            TargetPointer.SetParent(target.transform);
            TargetPointer.position = target.transform.position + TargetPointerOffset;
            return target;
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

        // 캐릭터의 버프&디버프 처리는 아래의 두가지로 보통 구현됩니다
        // 1. 버프를 주는쪽이 처리하는 케이스
        // 2. 받는 쪽이 처리하는 케이스
        // 2번 항목을 구현해볼겁니다.

        private void RefreshBuff()
        {
            // UnitStats 클래스의 SetBuff를 올바른 매개변수(테이블 수치)를
            // 가지고 호출 할 수 있도록만 처리해주면 됩니다.

            // 예외 처리
            // 지휘관과 지휘관이 있을 경우네는 서로 쌍방으로 버프를 주기 때문에
            // 지휘관은 건너 뜁니다.
            if (Def.Kind == UnitKind.Commander) return;
            // 여기 아래서 부터는 Sniper, Cannon 계열만 적용 됩니다
            
            // 변수 준비
            float atkPercent = 0f;
            float spdPercent = 0f;
            
            // 변수 연산 및 처리
            // : 지휘관을 찾아서 해당 지휘관 유닛의 버프 수치, 디버프 수치등을 알아서 판단해서 적용

            // 읽기전용 전체 유닛 List를 가지고 온다
            var units = UnitRegistry.All;

            for (int i = 0; i < units.Count; i++)
            {
                // 개체를 먼저 가져온다
                Unit other = units[i];
                
                // 지휘관이 아니라면 건너 뛴다
                
                if(other.Def.Kind != UnitKind.Commander) continue;
                // 아래서부터는 지휘관 개체일 경우 처리
                
                // 범위가 올바른지 판단을 합니다.
                // square 값(제곱)으로 빠르게 계산해줍니다
                float sqrRange = other.Def.BuffRange * other.Def.BuffRange;
                // other와 나의 거리의 제곱값을 가지고 온다
                float distance = (other.transform.position - transform.position).sqrMagnitude;
                // 거리가 범위보다 크다면(더 멀리있다면) 다음으로 Pass한다
                if (sqrRange < distance) continue;

                atkPercent += other.Def.BuffAtkPercent;
                spdPercent += other.Def.BuffSpeedPercent;
            }
            
            // 변수 적용
            Stats.SetBuff(atkPercent, spdPercent);
        }

        #endregion
    }
}