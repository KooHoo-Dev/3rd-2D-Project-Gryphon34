namespace Study.MiniDefence
{
    public class WaveDTO
    {
        public int ID;
        public int EnemyID;
        public int Count;
        public float Interval;
        public float Delay;
        public int RewardID;
    }
    
    [System.Serializable]
    public class WaveDef
    {
        public int ID;
        public EnemyDef Enemy;
        public int Count;
        public float Interval;
        public float Delay;
        public int RewardID;
    }
}