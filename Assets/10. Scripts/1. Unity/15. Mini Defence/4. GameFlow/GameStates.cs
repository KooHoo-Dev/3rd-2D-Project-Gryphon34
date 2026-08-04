using System;
using Jay.FSM;
using UnityEngine;

namespace Study.MiniDefence
{
    // 게임 상태는 어차피 게임 하나에만 쓸수있는 클래스기 때문에
    // 여기는 종속성을 신경 안써도 쌉가능입니다
    
    // IUpdatableState가 있는 이유는 준비상태에서 시간을 측정해서
    // 자동으로 InWave상태로 진입시키기 위해 있습니다.
    public class ReadyState : IGameFlowState, IUpdatableState
    {
        public GameStateId Id => GameStateId.Ready;
        public bool CanBuild => true;
        public bool CanCallWave => true;
        public bool CanSave => true;

        public float Remaining { get; private set; }

        public void Enter()
        {
            //Remaining = 60;
            Remaining = GameSettings.Instance.PrepDuration;
        }
        
        public void Tick(float deltaTime)
        {
            // 자동으로 0 최소값 세팅됨
            Remaining = Mathf.Max(0, Remaining - deltaTime);
        }

        public void Exit()
        {
            
        }
    }

    public class InWaveState : IGameFlowState
    {
        public GameStateId Id => GameStateId.InWave;
        public bool CanBuild => true;
        public bool CanCallWave => false;
        public bool CanSave => false;
        
        public void Enter() { }

        public void Exit() { }
    }

    public class VictoryState : IGameFlowState
    {
        public GameStateId Id => GameStateId.Victory;
        public bool CanBuild => false;
        public bool CanCallWave => false;
        public bool CanSave => false;

        public void Enter()
        {
            Time.timeScale = 0;
            UIManager.Instance.SetResult(true);
        }

        public void Exit() { }
    }

    public class DefeatState : IGameFlowState
    {
        public GameStateId Id => GameStateId.Defeat;
        public bool CanBuild => false;
        public bool CanCallWave => false;
        public bool CanSave => false;

        public void Enter()
        {
            Time.timeScale = 0;
            UIManager.Instance.SetResult(false);
        }

        public void Exit() {  }
    }
}