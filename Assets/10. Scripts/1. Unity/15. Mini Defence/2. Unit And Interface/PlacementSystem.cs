using System;
using Jay;
using UnityEngine;

namespace Study.MiniDefence
{
    // 설계는 객체의 역할을 나누는게 핵심입니다.
    // BoardMap         : 상태. 타일이 지금 어떤 상태인가? => 상태 보관
    // PlacementSystem  : 규칙(게임 로직). 그래서 스폰해도 되는가? => 규칙 판단
    
    // 유닛을 "놓고 / 팔고" 하는 규칙의 처리를 담당하는 녀석
    // 유저의 자원을 소모하거나 증가 시키는것(쓰고, 돌려주는) 일은
    // EconomyManager가 담당한다. 잔액을 변경한다거나 하지 않고
    // PlacementSystem은 그저 요청만 할 뿐.
    
    public class PlacementSystem : SingletonBase<PlacementSystem>
    {
        [SerializeField] private Transform unitRoot = null;

        #region Public Method

        // 지정한 타일에 유닛을 세운다. 실패하면 null을 반환
        public Unit Build(UnitDef def, Vector2Int coord)
        {
            // 규칙 흐름
            // 1 : 타일 규칙을 확인합니다
            // 2 : 비용을 확인합니다 (게임에 경제시스템 들어가면 구현)
            // 3 : 타일 중심에 유닛을 생성하고 초기화
            // 4 : 유닛이 점유했다고 보드 & 타일에 기록 합니다
            
            // 나중에 개발할때는 아래처럼 싱글톤 호출하지말고
            // 캐싱해서 쓰세요
            if (BoardMap.instance.IsBuildable(coord) == false) 
                return null;
            
            // 돈처리하는거 넣고, 돈없으면 return 때리기
            int cost = def.Cost;
            if (EconomyManager.instance.TrySpend(cost) == false)
            {
                Debug.Log($"[PlacementSystem] 광물이 부족합니다." +
                          $"필요 : {cost}, 보유 : {EconomyManager.Instance.Minerals}");
                return null;
            }
            
            Unit unit = Instantiate(def.Prefab, unitRoot);
            unit.transform.position = BoardMap.instance.CellToWorld(coord);
            BoardMap.instance.SetUnit(coord, unit);
            unit.Init(def, coord);
            return unit;
        }

        // 지정한 타일에 유닛을 제거한다
        public bool Remove(Vector2Int coord)
        {
            // 규칙 흐름
            // 1) 타일 확인합니다
            // 2) 타일 점유 해제 처리 합니다.
            // 3) 금액을 환불 받습니다 (게임에 경제시스템 들어가면 구현)

            Unit unit = BoardMap.instance.GetUnitOrNull(coord);
            if (unit == null) return false;
            
            BoardMap.instance.SetUnit(coord, null);
            // 자원 환불 처리
            Destroy(unit.gameObject);
            return true;
        }

        #endregion

        #region 미리 보기 기능

        // 임시 Unit 개체를 생성해서 미리보기 기능을 만들어 봅시다.
        
        [Header("미리보기 설정")]
        [SerializeField] [Range(0, 1)] private float previewAlpha = 0.7f;

        private Unit previewUnit;

        public event Action<Unit> UnitPlaced;
        public event Action<Unit> UnitRemoved;
        
        // 이 자리에 def 유닛을 세우면 어떤 모습인지 확인하기위해
        // 반투명 개체를 먼저 놓아본다. 유닛의 스폰 확정은 Build가 한다
        public void ShowPreview(UnitDef def, Vector2Int targetCoord)
        {
            ClearPreview();

            // 같은 개체를 하나 생성해서 unitRoot의 자식으로 넣어주고
            // previewUnit 변수에 할당해준다.
            previewUnit = Instantiate(def.Prefab, unitRoot);
            // 프리뷰 유닛의 위치에
            // BoardMap 셀 인덱스 targetCoord 좌표에 월드 포지션으로 대입해준다. 
            previewUnit.transform.position = BoardMap.instance.CellToWorld(targetCoord);
            previewUnit.gameObject.name = $"Preview_{def.Name}";

            // 프리뷰 개체의 동작을 정지시켜놓는다
            previewUnit.enabled = false;
            previewUnit.GetComponent<Animator>().enabled = false;

            // 프리뷰 개체를 반투명하게 만들어 준다
            SpriteRenderer[] spriteRenderers = 
                previewUnit.GetComponentsInChildren<SpriteRenderer>();

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                SpriteRenderer sr = spriteRenderers[i];
                Color color = sr.color; // sr의 컬러를 값 복사 해온다
                color.a = previewAlpha; // color의 알파값을 previewAlpha로 설정한다
                sr.color = color;       // color를 sr컬러에 값 복사 한다
            }
        }

        public void ClearPreview()
        {
            if(previewUnit == null) return;
            Destroy(previewUnit.gameObject);
            previewUnit = null;
        }

        #endregion
        
        // 판매시 돌려줄 금액.
        public int GetRefund(Unit unit)
            => Mathf.RoundToInt(unit.Def.Cost * GameSettings.Instance.RefundRate);

    }
}