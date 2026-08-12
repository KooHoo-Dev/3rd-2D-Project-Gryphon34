using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Study.MiniFarm
{
    // 농부에게 시킬 일 하나를 계속해서 명령하는 역할의 클래스
    
    // "한 칸 걷기", "괭이질하기" 처럼 시간이 걸리는 일을 객체로 만듭니다.
    // 이런 하나의 명령단위 클래스들을 만들면 이점이 3가지 정도 있습니다.
    //  - 행동을 나열 할 수 있다 : 큐같은 자료구조를 이용하면 순서대로 실행됨.
    //  - 행동을 취소 할 수 있다 : 아직 실행 안 한 것은 취소하면서 버릴 수 있음.
    //  - 섞을 수 있다. : "걷기 방향전환하기 걷기" 등등 작은 단위의 행동을 이어서 붙힐 수 있음

    // Tip : 교육용이어서 코루틴 썼음. 왠만하면 쓰지마세여. 다른 비동기 처리가 좋음
    
    public interface IFarmerCommand
    {
        IEnumerator Execute(FarmerController farmer);
        void Cancel();
    }
    
    // "목표 지점까지 한칸 걷는다"를 캡슐화한 커맨드 객체
    public class MoveStepCommand : IFarmerCommand
    {
        private Vector3 goal;
        private float speed;
        
        // 트윈의 종료를 제어하기 위해 멤버변수로 자리를 마련해 둡니다.
        private Tween tween;

        public MoveStepCommand(Vector3 goal, float speed) => Set(goal, speed);
        
        public void Set(Vector3 g, float s)
        {
            this.goal = g;
            this.speed = s;
        }
        
        public IEnumerator Execute(FarmerController farmer)
        {
            Transform body = farmer.transform;
            // 정면처리를 먼저 합니다.
            farmer.Face(goal - body.position);
            farmer.SetWalking(true);

            tween = body.DOMove(goal, 1 / speed)
                .SetEase(Ease.Linear);

            yield return tween.WaitForCompletion();
            // DOTween에는 다양한 대기 함수들이 있는데, 그 중에
            // .WaitForCompletion() 이 함수는
            // IEnumerator(코루틴)을 활용해서,
            // 트윈이 끝날때 까지 대기 할 수 있게 해주는 함수 입니다.

            tween = null;
        }

        // 트윈을 Kill해서 멈춰 버립니다.
        // tween은 외부에서 동작하기 때문에 StopCoroutine
        // 쓴다고 해서 멈추지 않는다는것 기억하십쇼.
        public void Cancel()
        {
            tween?.Kill();
            tween = null;
        }
    }
    
    // "행동 애니메이션을 재생하고, 정해진 순간에 실제 효과를 낸다"를
    // 캡슐화한 커맨드 객체
    
    // 효과를 트리거랑 동시에 내지 않는게 중요합니다.
    // Ex) 물주기 애니메이션이 있으면
    //  물주기 애니메이션 시작 => 30%정도 진행되면 실제 효과 적용 => 물주기 애니메이션 종료
    
    public class FarmerActionCommand : IFarmerCommand
    {
        private FarmerAction action;
        private float windUp;       // 실제 효과 적용시까지 잠깐 기다리는 용도
        private float duration;      
        private Action onHit;       // 콜백용

        public FarmerActionCommand(FarmerAction action, float windUp, 
            float duration, Action onHit)
        {
            this.action = action;
            this.windUp = windUp;
            this.duration = duration;
            this.onHit = onHit;
        }
        
        public IEnumerator Execute(FarmerController farmer)
        {
            // 걷는 애니메이션을 멈추게하고
            farmer.SetWalking(false);
            farmer.Play(action);

            // 아래 코드는 변경 예정
            yield return new WaitForSeconds(windUp);
            onHit?.Invoke();
            yield return new WaitForSeconds(duration - windUp);
        }

        public void Cancel()
        {
            // 간단한 애니메이션 재생이라 무를 수도 없고 할게 없음.
        }
    }

    // 명령을 줄 세워 두고 하나씩 실행하는 객체. (커맨드 패턴의 실행기)\
    
    // 우클릭하면 경로를 구해서 "한 칸 걷기" 명령으로 쪼개서 줄 세웁니다.
    // 걷는 도중 다시 우클릭하면 남은 줄을 통째로 버리고 새로 세웁니다.
    // 명령을 객체 단위로 캡슐화 했기 때문에 이 취소가 "줄을 비운다"는
    // 개념으로 처리됩니다.
    
    // 이 컴포넌트가 하지 않는 일
    // - 길을 찾지 않습니다 : PathFinder가 한다
    // - 애니메이터를 만지지 않는다 : FarmerController가 한다
    // - 규칙을 따지지 않는다 
    [RequireComponent(typeof(FarmerController))]
    public class FarmerCommander : MonoBehaviour
    {
        [Header("Ref")] 
        [SerializeField] private PathFinder pathFinder;
        [SerializeField] private Camera viewCamera;

        [Header("Settings")] 
        // 임시 스피드 값
        [SerializeField] private float moveSpeed = 3f;

        private FarmerController farmerController;
        private readonly List<IFarmerCommand> queue = new List<IFarmerCommand>();
        // readonly
        // : 빌드할때 속도 이점이 있고, 멀티쓰레딩할때 확실한 성능적 효과가 있음.
        //  기계어로 빌드 될때 효율적인 데이터 타입으로 변환이 가능.
        
        private IFarmerCommand currentCommand;
        private Coroutine running;
        // Coroutine
        // : 구현체가 아니라 돌릴수 있는 객체 (Handle)
        //  실행을 시키거나 중단하거나 언제까지 기다려야할지 등.
        //  "코루틴을 제어하는 객체" 라고 생각하시면 됩니다.
        // 실제 코루틴의 객체는 "C#의 이터레이터인 IEnumerator가 맞습니다"
        
        // Tip !
        // : 돌리는 녀석이 참조타입(힙 객체)이어서 사실 코루틴 하나 돌리고,
        //  하나 종료하면 가비지(Garbage)가 발생합니다.

        private void Awake()
        {
            farmerController = GetComponent<FarmerController>();
            if(viewCamera == null) viewCamera = Camera.main;
        }

        private void Update()
        {
            if (pathFinder == null || pathFinder.IsReady == false) return;

            if (SimpleInput.GetMouseButtonDown(SimpleInput.MouseButton.Right))
            {
                Vector2 mousePos = SimpleInput.GetMousePosition();
                Vector3 world = viewCamera.ScreenToWorldPoint(mousePos);
                world.z = 0.0f;
                MoveTo(world);
            }
        }

        private void OnDisable()
        {
            CancelAll();
        }

        # region Public Methods
        
        // 지정한 월드 지점까지 걸어간다.
        // PathFinder를 이용해서 웨이포인트를 가져오고 해당 포인트를 이용해서
        // MoveStepCommand를 동적으로 생성한다.
        public bool MoveTo(Vector3 goal)
        {
            CancelAll();
            
            // 웨이포인트를 가져온다 (내 위치, 목적지)
            List<Vector3> waypoints = 
                pathFinder.FindWaypointsOrNull(transform.position, goal);

            if (waypoints == null || waypoints.Count == 0) return false;

            // 웨이포인트를 가지고 MoveStepCommand를 생성한다
            for (int i = 0; i < waypoints.Count; i++)
            {
                queue.Add(new MoveStepCommand(waypoints[i], moveSpeed));
            }
            
            StartRunning();
            
            return true;
        }

        public void CancelAll()
        {
            queue.Clear();
            
            // 동작하는 RunQueue() 코루틴이 있다면
            if(running != null)
            {
                StopCoroutine(running); // 멈추고
                running = null;         // 비워준다
            }
            
            currentCommand?.Cancel();
            currentCommand = null;
            
            farmerController.SetWalking(false);
        }

        // 걷기 애니메이션 빼고 다른 애니메이션 집어 넣을때 사용함.
        public void Enqueue(IFarmerCommand command)
        {
            queue.Add(command);
            StartRunning();
        }
        
        # endregion

        #region Private Methods

        private void StartRunning()
        {
            // 동작중인 RunQueue() 코루틴이 있다면 Pass
            if (running == null) running = StartCoroutine(RunQueue());
        }

        // queue가 빌때까지 앞에서부터 하나씩 꺼내서 실행한다.
        // 명령마다 코루틴을 따로 돌리지 않고, 여기서 한 줄기로 이어 붙인다
        private IEnumerator RunQueue()
        {
            while (queue.Count > 0)
            {
                // 변수명이 queue지 실제로는 List임에 유의
                currentCommand = queue[0];
                queue.RemoveAt(0);
                
                // StartCoroutine을 실행하는게 아님
                yield return currentCommand.Execute(farmerController);
                currentCommand = null;
            }
            
            farmerController.SetWalking(false);
            running = null;
        }

        #endregion
    }
}