using Jay;
using UnityEngine;

namespace Study.MiniDefence
{
    // 설계는 객체의 역할을 나누는게 핵심입니다.
    // BoardMap         : 상태. 타일이 지금 어떤 상태인가? => 상태 보관
    // PlacementSystem  : 규칙(게임 로직). 그래서 스폰해도 되는가? => 규칙 판단
    
    // MapData는 저장된 "정보"일 뿐이고, SquareGrid는 좌표 "계산"을 담당하는 자료구조임
    // 이 둘을 이어서 "지금 이 씬에서 실제로 굴러가는 게임보드"로 만들어주는 역할의 객체가
    // BoardMap임. 게임 보드는 씬에 하나만 존재해야함으로 싱글톤으로 선언
    
    public class BoardMap : SingletonBase<BoardMap>
    {
        [SerializeField] private MapData mapData;

        private SquareGrid grid;
        
        public MapData Map => mapData;
        public SquareGrid Grid => grid;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            if (mapData == null)
            {
                Debug.LogError($"[BoardMap] mapData is null");
                return;
            }
            
            grid = mapData.ToGrid();
            Debug.Log($"Width : {grid.Width}, Height : {grid.Height}" );
        }
        
        // SquareGrid의 함수를 BoardMap을 거쳐서 이용할 수 있지만,
        // BoardMap에서 검사해야할 내용들 추가한 함수들까지 포괄할 수있게
        // 다시 함수를 감싸줍니다. ( 래핑한다~ 어댑터~) 
        # region 좌표 변환 함수들 감싸주기 
        
        // 한줄 짜리 연결은 아래처럼 하면 컴파일시 인라인 가능성이 올라감.
        // 권장하는 표현. 
        public Vector2 WorldToCell(Vector3 worldPosition) 
            => grid.WorldToCell(worldPosition);
        
        // 개발 도중에는 아래처럼 표현해놓는것을 추천함
        public Vector3 CellToWorld(Vector2Int coord)
        {
            return grid.CellToWorld(coord);
        }

        // SquareGrid.WorldToCell은 격자 밖 좌표를
        // 가장자리로 Clamp하여 처리함.
        // 그래서 "보드 바깥을 클릭했다"를 구분하려면
        // 월드좌표로 먼저 범위를 확인해줘야 함. 그래서 아래 함수를 사용
        public bool Contains(Vector3 worldPosition)
        {
            // 매개변수인 worldPosition이 보드 영역 안인가?
            Vector2 min = grid.Origin;
            Vector2 max = min + new Vector2(grid.Width, grid.Height) * grid.CellSize;
            
            return (min.x <= worldPosition.x) && (worldPosition.x <= max.x) && 
                   (min.y <= worldPosition.y) && (worldPosition.y <= max.y);
        }
        
        #endregion
        
        # region 타일 조회
        
        public Tile GetTileOrNull(Vector2Int coord)
         => grid.GetTileOrNull(coord);
        
        // 마우스 위치로 타일을 찾아야 함으로 새로운 기능 추가한
        // 함수를 만들어 준다. (입력 처리에서 사용합니다)
        public Tile GetTileAtWorldOrNull(Vector3 worldPosition)
        {
            if (Contains(worldPosition) == false) return null;
            return grid.GetTileOrNull(grid.WorldToCell(worldPosition));
        }
        
        # endregion
        
        # region 점유(유닛 배치) 상태

        public bool IsBuildable(Vector2Int coord)
        {
            Tile tile = GetTileOrNull(coord);
            if(tile == null) return false;
            return tile.IsBuildable();
        }

        // 게임에서 특정 좌표의 유닛을 확인할 수 있는 기능을
        // 맹글어 준다.
        public Unit GetUnitOrNull(Vector2Int coord)
        {
            Tile tile = GetTileOrNull(coord);
            return tile?.Unit; 
            // tile은 없을리가 없고, Unit에 들어간게 없을수는 있음
            // Unit 상태(null or 진짜 객체)만 반환하면 땡
        }

        // PlacementSystem에서 유닛을 생성하거나 제거할 때
        // 아래 함수를 이용한다
        // 생성 : 매개변수 unit 객체 넣어줌
        // 제거 : 매개변수 unit에 null 넣어줌
        public void SetUnit(Vector2Int coord, Unit unit)
        {
            Tile tile = GetTileOrNull(coord);
            tile.Unit = unit;
        }
        
        #endregion
    }
}