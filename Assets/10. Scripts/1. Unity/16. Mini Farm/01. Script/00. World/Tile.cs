using UnityEngine;

namespace Study.MiniFarm
{
    // 그리드의 한 칸.
    public class Tile
    {
        public Vector2Int Coord { get; }

        public TileDef Def { get; private set; }

        public TileType Type => Def.TileType;
        public int MoveCost => Def.MoveCost;
        public bool IsWalkable => Def.Walkable;
        public bool IsTillable => Def.Tillable;

        public Tile(Vector2Int coord, TileDef def)
        {
            Coord = coord;
            Def = def;
        }

        public void ChangeType(TileDef def)
        {
            Def = def;
        }

        #region 길찾기 탐색 상태

        // 출발 타일에서 여기까지 실제로 지나온 비용의 합.
        public int GCost;

        // 여기서 목표 타일까지의 예상 비용. (휴리스틱)
        public int HCost;

        // 이 타일을 거치는 경로의 총 예상 비용.
        public int FCost => GCost + HCost;

        // 경로를 되짚어 갈 때 쓰는, 여기로 오기 직전의 타일.
        public Tile Parent;

        // 탐색을 시작하기 전에 모든 타일에 대해 부른다.
        // GCost 를 최댓값으로 두는 이유는 "아직 와 본 적 없다"를 표현하기 위해서.
        public void ResetPathCosts()
        {
            GCost = int.MaxValue;
            HCost = 0;
            Parent = null;
        }

        #endregion
    }
}
