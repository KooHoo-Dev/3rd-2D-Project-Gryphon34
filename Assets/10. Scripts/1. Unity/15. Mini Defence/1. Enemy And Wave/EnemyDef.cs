using UnityEngine;

namespace Study.MiniDefence
{
    public class EnemyDTO
    {
        public int ID;
        public string PrefabKey;
        public string SpriteKey;
        public int MaxHp;
        public float Speed;    // 초당 얼마나의 타일을 이동할 수 있는지
        public int Armor;      // 고정 감산 
        public float Resist;   // 비율 감산 (0~1)
        public int KillReward;
        public int LeakDamage; // 목표 도달 시 기본 체력 감소량
    }
    
    [System.Serializable]
    public class EnemyDef
    {
        public int ID;
        public Enemy Prefab;
        public Sprite Sprite;
        public int MaxHp;
        public float Speed;
        public int Armor;      // 고정 감산 
        public float Resist;   // 비율 감산 (0~1)
        public int KillReward;
        public int LeakDamage; // 목표 도달 시 기본 체력 감소량
    }
}