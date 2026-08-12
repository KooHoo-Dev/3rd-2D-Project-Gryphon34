using System;
using DG.Tweening;
using Jay;
using UnityEngine;

namespace Study.MiniFarm
{
    // 농장 안에서 흐르는 시계. 하루는 굴리고, 멈추고, 빨리 감는다.
    // 이 게임에는 시계가 두 개 있습니다.
    // - 게임 시간(WorldClock)      : 작물 성장, 농부 이동, 시세 갱신
    // - 실제 시간(Engine, Device)  : UI, 커서, 카메라, 메뉴 등
    // 보통 일시정지가 있는 게임들은 엔진의 시간을 직접 사용하지 않고
    // 따로 정의를 해줍니다.
    // 이럴경우, 아래와 같은 개발 규칙이 추가됩니다.
    // - 게임 로직은 Time.deltaTime을 쓰지 않는다. 대신 WorldClock의 DeltaTime을 사용한다.
    public class WorldClock : SingletonBase<WorldClock>
    {
        [Header("Settings")] 
        private float tickInterval = 1f; // 알림 주기 역할을 하는 필드

        // 며칠째인가. 1일차부터 시작한다
        public int Day { get; private set; } = 1;
        // 하루 안에서 얼마나 진행되었는지. 0이 아침, 1이 밤
        public float TimeOfDay { get; private set; } = 1;
        // 지금 고른 배속 단계. int인 이유는 외부파일의 배열 데이터 인덱스로 사용함
        public int ScaleStep { get; private set; } = 1;

        public float Scale => settings.TimeScales[ScaleStep]; 
        
        public bool IsPaused => Scale <= 0f;

        // 게임 로직이 써야 하는 시간 간격
        // : Time.deltaTime이 아니라 unscaledDeltaTime을 사용.
        //   Time.timeScale은 건드리지 않기로 했으므로, 누구가 그것을
        //   바꾸더라도 흔들리지 않는 시계가 필요하다.
        //   배속은 우리가 직접
        public float DeltaTime =>Time.unscaledDeltaTime*Scale;

        // 마지막날 계산용 필드
        public bool IsFinished { get; private set; } = false;
        
        #region Events(델리게이트)

        // 게임시간이 tickInterval 만큼 흐를때마다 콜백
        public event Action OnTick;
        // 하루의 시작(새날이 밝았다!)
        public event Action<int> OnDayStarted;
        // 하루의 종료
        public event Action<int> OnDayEnded;
        // Pause의 상태 변화
        public event Action<bool> OnPauseChanged;

        #endregion
        
        // 디폴트 상수 두개 선언
        private const int PAUSED_STEP = 0;
        private const int DEFAULT_STEP = 1;

        private GameSettings settings;
        private float tickTimer;
        private int lasRunningStep = DEFAULT_STEP;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            // 게임 세팅의 인스턴스를 캐싱해준다
            settings = GameSettings.Instance;
            ScaleStep = DEFAULT_STEP;
            // DOTween의 타임 스케일을 WorldClock의 스케일로 바꿔준다
            DOTween.timeScale = Scale;
        }

        private void Start()
        {
            // 첫 프레임이 동작하면 첫날 아침을 일단 알려준다.
            // 기록된 데이터(사용자 데이터)를 통해서 오늘이 몇일인지 알려주게 된다.
            OnDayStarted?.Invoke(Day);
        }

        private void Update()
        {
            float delta = DeltaTime;

            if (delta <= 0f) return;
            AdvanceTick(delta);
            AdvanceDay(delta);
        }
        
        #region 시간 진행

        private void AdvanceTick(float delta)
        {
            // 틱 타이머를 갱신해주고
            tickTimer += delta;
            // 임계점에 도달하지 않았으면 return
            if (tickTimer < tickInterval) return;

            // 틱타이머가 설정된 tickIntercal 도달했으므로 빼주고(0해도 됨)
            tickTimer = 0; // 혹시 몰라서 넣어놓음 : tickTimer -= tickInterval;
            // OnTick을 구독하는 객체들에게 알려준다
            OnTick?.Invoke();
        }

        public void AdvanceDay(float delta)
        {
            TimeOfDay += delta / settings.DayLengthSeconds;
            if (TimeOfDay < 1f) return;

            TimeOfDay = 0f; // 혹시 몰라서 남겨둠 : TimeOfDay -=1f;
            OnDayEnded?.Invoke(Day); // 현재의 Day가 종료되었다!

            // 게임 종료체크를 해준다
            if (Day >= settings.TotalDays)
            {
                IsFinished = true;
                return;
            }
            else
            {
                Day++;
                OnDayStarted?.Invoke(Day);
            }
            
            
        }
        
        #endregion
        
        #region 배속

        // 배속 단계를 설정해준다. GameSettings의 TimeScales를 참고하며 
        // 인덱스를 변경해서 배속 단계를 바꾼다
        public void SetScaleStep(int step)
        {
            if (step < 0 || settings.TimeScales.Length <= step) return;
            if (step == ScaleStep) return;
            
            bool wasPauesd = IsPaused;
            
            if (step == PAUSED_STEP) lasRunningStep = ScaleStep;
            
            
            ScaleStep = step;
            DOTween.timeScale = Scale;
            
            // 정지된 상태에서 => 정지가 풀린거 
            if(wasPauesd) OnPauseChanged.Invoke(IsPaused);
        }
        
        public void Pause() => SetScaleStep(PAUSED_STEP);
        public void Resume() => SetScaleStep(lasRunningStep);

        public void TogglePause()
        {
            if (IsPaused) Resume();
            else Pause();
        }
        
        #endregion
    }
}