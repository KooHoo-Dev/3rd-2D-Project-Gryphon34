using Jay.FSM;

namespace Study.MiniDefence
{
    public enum GameStateId
    {
        Ready,
        InWave,
        Victory,
        Defeat,
    }
    
    public interface IGameFlowState : IState
    {
        GameStateId Id { get; }

        bool CanBuild { get; }      // 유닛을 세우거나 팔 수 있는가
        bool CanCallWave { get; }   // 다음 웨이브를 부를 수 있는가
        bool CanSave { get; }       // 저장할 수 있는가 (기획: 웨이브 사이에서만)
    }
}