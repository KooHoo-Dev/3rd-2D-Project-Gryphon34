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
            Unit unit = Instantiate(def.Prefab, unitRoot);
            unit.transform.position = BoardMap.instance.CellToWorld(coord);
            BoardMap.instance.SetUnit(coord, unit);
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

        
    }
}