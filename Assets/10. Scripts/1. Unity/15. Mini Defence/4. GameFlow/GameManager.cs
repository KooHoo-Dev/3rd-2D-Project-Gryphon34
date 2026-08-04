using System;
using Jay;
using Jay.FSM;
using UnityEngine;

namespace Study.MiniDefence
{
    // 현재 우리가 만들고 있는 디펜스게임은 몇몇 큰 흐름이 있습니다
    // 1. Ready : 유닛 구매, 판매 기능등이 활성화 됩니다.
    //            게임을 시작하거나, 웨이브를 클리어하면 진입
    // 2. InWave : 웨이브를 진행하는 단계.
    //           Ready상태에서 버튼을 눌러서 진입할 수 있습니다
    // 3. Defeat : 체력이 다 없어져서 패배하는 단계 입니다.
    // 4. Victory : 3웨이브를 클리어하면 진입하는. 승리 단계
    
    public class GameManager : SingletonBase<GameManager>
    {
        # region 상태 관련 필드
        
        private StateMachine<IGameFlowState> machine = new();

        // 준비상태는 남은 준비 시간을 물어봐야하 하므로 따로 들고 있는다.
        private ReadyState readyState = new();

        public IGameFlowState State => machine.CurrentState;
        public GameStateId StateId => machine.CurrentState.Id;

        // 남은 준비 시간을 반환합니다. Ready상태가 아니라면 무조건 0을 반환합니다.
        public float PrepRemaining
            => machine.IsCurrent<ReadyState>() ? readyState.Remaining : 0f;
        
        public event Action<IGameFlowState> StateChanged;
        
        # endregion
        
        // 목숨
        public int Lives { get; private set; }
        public event Action<int> LivesChanged;

        protected override void OnInitialize()
        {
            Lives = GameSettings.Instance.StartingLives;
            
            // 머신에 상태를 넣어준다
            machine.AddRange(new IGameFlowState[]
            {
                readyState,
                new InWaveState(),
                new VictoryState(),
                new DefeatState()
            });

            // 머신의 상태 변환 이벤트를 연결 해준다
            machine.OnStateChanged += InvokeStatChanged;

            void InvokeStatChanged(IGameFlowState prev, IGameFlowState next)
            {
                Debug.Log($"[StateMachine] {prev?.Id} -> {next.Id}");
                StateChanged?.Invoke(next);
            }
            
            // 머신의 상태를 ReadyState로 바꿔준다
            machine.ChangeState<ReadyState>();
        }

        private void Update()
        {
            // 유니티엔진의 업데이트 주기에 맞춰서
            // 머신의 Tick을 호출해줍니다
            machine.Tick(Time.deltaTime);
        }

        private void OnEnable()
        {
            WaveManager.Instance.WaveChanged += HandleWaveChanged;
            WaveManager.Instance.WaveCleared += HandleWaveCleared;
            WaveManager.Instance.EnemyReachedGoal += HandleEnemyLeaked;
        }

        private void OnDisable()
        {
            if (WaveManager.IsInitialized == false) return;
            WaveManager.Instance.WaveChanged -= HandleWaveChanged;
            WaveManager.Instance.WaveCleared -= HandleWaveCleared;
            WaveManager.Instance.EnemyReachedGoal -= HandleEnemyLeaked;
        }

        private void HandleWaveChanged(int waveNo)
        {
            machine.ChangeState<InWaveState>();
        }

        private void HandleWaveCleared(WaveDef waveDef)
        {
            int hasNextWave = WaveManager.Instance.CurrentWaveNo;
            
            // 승리 판정
            if (DataManager.Instance.Waves.Count <= hasNextWave)
                machine.ChangeState<VictoryState>();
            else
                machine.ChangeState<ReadyState>();
            
            //PS : 남은 웨이브에 다라 승리/준비 단계로 전환하는 내용이 들어가면 될것 같습니다
        }

        private void HandleEnemyLeaked(Transform lastPosition, EnemyDef enemyDef)
        {
            LoseLives(enemyDef.LeakDamage);
        }

        public void GainLives(int lives)
        {
            Lives = lives;
            LivesChanged?.Invoke(Lives);
        }

        public void LoseLives(int amount)
        {
            Lives -= amount;
            LivesChanged?.Invoke(Lives);
            if(Lives <= 0) machine.ChangeState<DefeatState>();
        }
        
        
    }
}