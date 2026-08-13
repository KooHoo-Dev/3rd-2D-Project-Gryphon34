using System;
using DG.Tweening;
using UnityEngine;

namespace Study.MiniFarm
{
    // 작물 하나의 데이터 + 컴포넌트
    // - 며칠 만에 자라는가?
    // - 얼마에 파는가?
    // - 어떻게 보이는가?
    
    // DTO -> Def 구조가 더 좋지만
    // 너무 어렵다는 피드백이 있어서 그냥 깡 스크립트로 만듭니다.
    public class Crop : MonoBehaviour
    {
        [Header("작물 데이터")]
        [field:SerializeField] public int ID { get; private set; }             // 작물의 ID(식별자)
        [field:SerializeField] public string CropName { get; private set; }    // 작물명
        [field:SerializeField] public int SeedPrice { get; private set; }      // 씨앗 가격
        [field:SerializeField] public int GrowDays { get; private set; }       // 다 자랄때까지 필요한 성장 일수
        [field:SerializeField] public int WaterNeed { get; private set; }      // 다 자랄때까지 필요한 물을 줘야 하는 횟수?
        [field:SerializeField] public int Yield { get; private set; }          // 수확량
        [field:SerializeField] public int HarvestItemID { get; private set; }  // 수확했을때 얻는 아이템의 ID
        
        [Header("---")]
        [SerializeField] private Sprite[] sprites;
        // waterTint는 물에 젖은것을 표현하기 위해 사용함
        [SerializeField] private Color waterTint = new Color(0.8f,0.85f, 0.9f);

        private SpriteRenderer spriteRenderer;


        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            Refresh();
        }

        public void Restore()
        {
            
        }
        
        # region 성장 관련 함수들

        public bool WateredToDay { get; private set; } = false;
        public int WateredCount { get; private set; } = 0;

        public bool Water()
        {
            if (WateredToDay) return false;
            WateredToDay = true;
            WateredCount++;

            Refresh();
            return true;
        }

        // 현재 작물이 며칠 정도를 자랐는지 체크하는 용도의 변수
        // 성장 단계
        public int GrowthStage { get; private set; } = 0;
        
        // 다 자랐는지 체크하는 필드
        public bool IsGrown => GrowthStage >= GrowDays; 
        
        // 성장 시키는 함수. 외부에서 여러번 호출될 가능성이 있다.
        public void Grow()
        {
            // 이전에 다 자랐는지 체크하기 위한 지역변수
            bool wasGrown = IsGrown;
            // if (WateredToDay && IsGrown == false) GrowthStage++;
            if (IsGrown == false) GrowthStage++;
            WateredToDay = false;
            Refresh();
            
            // 방금 다 자랐는지만 체크해줘서. 특별한 효과를 준다
            if (wasGrown == false && IsGrown) // 조건식이 true라면 이번 Grow 호출때 다 자란거임
            {
                transform.DOPunchScale(Vector3.one * 0.3f, 0.3f);
            }
        }
        
        # endregion
        
        # region 상태 표현

        public void Refresh()
        {
            spriteRenderer.sprite = sprites[StageIndex()];
        }

        private int StageIndex()
        {
            int last = sprites.Length - 1;
            if (IsGrown) return last;

            float ratio = (float)GrowthStage / GrowDays;
            int result = Mathf.FloorToInt(ratio * last);
            return Mathf.Clamp(result, 0, sprites.Length - 1);
        }
        
        #endregion
        
    }
}