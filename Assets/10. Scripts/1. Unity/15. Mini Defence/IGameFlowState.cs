namespace Study.MiniDefence
{
    public enum GameStateId
    {
        Ready,
        InWave,
        Victory,
        Defeat,
    }
    
    public class IGameFlowState
    {
        private GameStateId Id { get; }

        private bool CanBuild { get; }
        bool CanCallWave { get; }
        
    }
}