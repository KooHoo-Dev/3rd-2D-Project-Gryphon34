using UnityEngine;

namespace Study.MiniDefence
{
    // 유닛 한 기의 "지금 스탯". 기본값(UnitDef)에 수정자를 얹어
    //  최종값을 계산을 한다
    // : 표의 기본값 + 수정값(지휘관 강화 등등) = 실제 사용값(적용 값)
    // 값을 보관하지 않고 물어볼 때마다 계산한다(요청할때마다)
    
    public class UnitStats
    {
        private readonly UnitDef def;
        
        public int Damage => Mathf.RoundToInt(def.Damage * (1f + BonusAtkPercent));
        public float Range => def.Range;
        public float CritChance => def.CritChance;
        // 공격 속도(공격과 공격사이의 시간 간격으로 표현됨)
        public float AttackInterval => def.AttackInterval / (1f + BonusSpeedPercent);

        // 아래 두개가 실제 사용되는 버프 퍼센트 데이터
        public float BonusAtkPercent { get; private set; }
        public float BonusSpeedPercent { get; private set; }
        
        // 생성자 람다식 표현
        public UnitStats(UnitDef def) => this.def = def;

        public void SetBuff(float attackPercent, float speedPercent)
        {
            BonusAtkPercent = attackPercent;
            BonusSpeedPercent = speedPercent;
        }
    }
}