using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using Study.MiniDefence._3._Combat;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Study.MiniDefence
{
    public class Enemy : MonoBehaviour
    {
        private static readonly int Dead = Animator.StringToHash("Dead");
        private static readonly int DeadAnimation = Animator.StringToHash("DeadAnimation");
        private static readonly int IsMove = Animator.StringToHash("IsMove");
        private EnemyDef refDef;
        private Queue<Vector3> moveQueue;
        private Vector3 currentGoal;
        

        // 목표지점에 도착했을때 알림을 통지할 델리게이트.
        // 외부 클래스에서는 해당 델리게이트를 구독해서 알맞은 처리를 진행한다
        public event Action<Enemy> GoalReached;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            MoveNext();
        }

        private void OnEnable()
        {
            EnemyRegistry.Register(this);
        }

        private void OnDisable()
        {
            EnemyRegistry.Unregister(this);
            // Enemy가 만약에 체력이 닳아서 죽게된다면 진행중인 Tween을 없애준다
            // (자동으로 되는지 모르겠음. 아마 안될껄?)
            transform.DOKill();
        }

        public void Init(EnemyDef def, List<Vector3> waypoints)
        {
            // 매개변수로 전달된 EnemyDef 객체(레퍼런스 타입)를 캐싱한다
            refDef = def;

            moveQueue = new Queue<Vector3>();
            for (int i = 0; i < waypoints.Count; i++)
                moveQueue.Enqueue(waypoints[i]);
            // moveQueue에서 가장 첫번째 값(waypoints[0]) 위치로 이동한다 
            transform.position = moveQueue.Dequeue();

            totalPointCount = waypoints.Count;
            passedPointCount = 0;
            segmentStart = transform.position;

            CurrentHp = refDef.MaxHp;
        }

        private void MoveNext()
        {
            if (moveQueue.Count == 0)
            {
                // 도착 처리
                GoalReached?.Invoke(this); //내가(this) 목표지점에 도착했음을 알려준다
                gameObject.SetActive(false);
                return;
            }
            // 이동할 웨이포인트가 남았다면
            // moveQueue에서 하나를 뽑아서 목적지를 갱신하고
            currentGoal = moveQueue.Dequeue();
            // 이동 처리를 한다
            transform.DOMove(currentGoal, 1/refDef.Speed)
                .SetEase(Ease.Linear)
                .OnComplete(OnSegmentComplete);
            animator.SetBool(IsMove, true);
            
        }
        
        // ------ 아래는 다른 객체와의 상호작용 및 구체적 구현 부분 -----
        
        private Animator animator;

        private int totalPointCount;    // 경로 지점 수
        private int passedPointCount;   // 지나온 지점 수
        private Vector3 segmentStart;   // 지금 이동중인 구간의 월드 출발 포인트

        // 목표 지점까지 얼마나 왔는가? (0~1)
        // "선두, 후미" 등 조준을 계산할때 사용합니다
        public float Progress
        {
            get
            {
                if (totalPointCount <= 1) return 1f;
                float segmentLength  = Vector3.Distance(segmentStart, currentGoal);
                float remainDistance = Vector3.Distance(transform.position, currentGoal);
                // ratio 는 "비"의 영어 표기
                // 1:3 에서 ":"
                float ratio = 1f - Mathf.Clamp01(remainDistance / segmentLength);
                // float ratio = (segmentLength < 0.001f) ?
                //         1f : 1f - Mathf.Clamp01(remainDistance / segmentLength);
                
                float result = Mathf.Clamp01((passedPointCount + ratio) / (totalPointCount -1));
                return result;
            }
        }

        #region 전투

        public int CurrentHp { get; private set; }
        public bool IsDead => (CurrentHp <= 0);

        // 피격 연출이나 죽음 연출 등을 위해
        // 이벤트를 선언해 놓는다
        public event Action<Enemy, int, bool> Damaged;
        public event Action<Enemy> Died;

        // CombatSystem 안쓰고 직접 구현합니다.
        // 콜라이더도 안들어가있고 그냥 바로 호출하는 구조입니다.

        // 적용하기 전에 시뮬레이션을 해봅니다
        public int CalculateFinalDamage(int damage)
        {
            // 때리는 쪽은 "얼마를 보낼지" damage로 전달하고 (크리티컬도 포함)
            // 방어와 저항은 맞는 쪽에서 계산 한다.
            int afterArmor = Mathf.Max(0, damage - refDef.Armor);
            return Mathf.RoundToInt(afterArmor * (1f - refDef.Resist));
        }

        // 디폴트 매개변수 ?
        // : 매개변수에 기본값을 설정할 수 있다.
        // 아래처럼 (자료형 매개변수명 = 기본값) 형태로 함수를 정의하면
        // 호출하는 쪽에서 매개변수를 전달하지 않으면 기본값이 대입되어
        // 함수가 실행된다.
        public void TakeDamage(int damage, bool isCritical = false)
        {
            if(IsDead) return;
            // 먼저 시뮬레이이션 해보고
            int finalDamage = CalculateFinalDamage(damage);
            // 적용한다
            CurrentHp = Mathf.Max(0, CurrentHp - finalDamage); // 0이하처리 보정
            // 알린다 
            Damaged?.Invoke(this, finalDamage, isCritical);
            if(IsDead) Die(); 
        }

        private void Die()
        {
            EconomyManager.Instance.Earn(refDef.KillReward); // 처치 보상 적용하고\
            // 알린다
            Died?.Invoke(this);
            // 움직이지 않게 해준다
            transform.DOKill();
            
            // AnyState를 호출하기 전에 죽는모션을 랜덤으로 골라서
            // .SetInteger()를 이용해 설정해 준다
            int randNum = UnityEngine.Random.Range(1, 4); // 1,2,3 중 하나
            animator.SetInteger(DeadAnimation, randNum);
            // 호출한다
            animator.SetTrigger(Dead);
            StartCoroutine(DeadRoutine());
        }

        private IEnumerator DeadRoutine()
        {
            yield return new WaitForSeconds(2.0f);
            gameObject.SetActive(false);
        }

        #endregion

        // MoveNext함수의 DoMove 처리중 OnComplete 부분에 넣어준다
        // .OnComplete(OnSegmentComplete)
        private void OnSegmentComplete()
        {
            passedPointCount++;
            MoveNext();
        }
    }
}