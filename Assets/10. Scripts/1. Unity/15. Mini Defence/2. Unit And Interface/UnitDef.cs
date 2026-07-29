using UnityEngine;


namespace Study.MiniDefence
{
    // kind가 아니라 type이 맞는 표현인데
    // type이 혼동될까봐 바꿨습니다.
    // ai가 UnitType 이런형태로 대답하는게 맞는 표현이라고 저는 생각합니다
    public enum UnitKind
    {
        Sniper,     // 저격수
        Cannon,     // 포격병 : 광역 데미지 입히는 녀석
        Commander,  // 지휘관 : 약한 공격, 주변 아군 강화
    }

    public enum TargetingKind
    {
        First,      // 선두
        Last,       // 후미
        Nearest,    // 가장 가까운
        Strongest,  // 체력이 제일 높은
        Weakest,    // 체력이 제일 낮은
    }

    public class UnitDTO
    {
        public int ID;
        public string Name;
        public string PrefabKey;
        public string SpriteKey;
        public UnitKind Kind;
        public TargetingKind Targeting;
        public int Cost;
        public int Damage;
        public float Range;
        public float AttackInterval;
        public float CritChance;
        public float SplashRadius;
        public int MaxTargets;
        public float BuffRange;
        public float BuffAtkPercent;
        public float BuffSpeedPercent;
    }
    
    [System.Serializable]
    public class UnitDef
    {
        public int ID;
        public string Name;
        public Unit Prefab;
        public Sprite Sprite;
        public UnitKind Kind;
        public TargetingKind Targeting;
        
        public int Cost;
        public int Damage;
        public float Range;
        public float AttackInterval;
        public float CritChance;
        
        public float SplashRadius;
        public int MaxTargets;

        public float BuffRange;
        public float BuffAtkPercent;
        public float BuffSpeedPercent;
    }
}