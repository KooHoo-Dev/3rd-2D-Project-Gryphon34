
using UnityEngine;

namespace Study.MiniDefence
{
    // 타일의 종류
    // 건설 패드 : 유닛 배치 가능
    // 통로 : 적 이동 경로. 배치 불가
    // 스폰 타일 : 적이 등장. 배치 불가
    // 목표 타일	: 적의 도착점. 배치 불가

    public enum TileKind
    {
        Buildable,  // 건설 패드
        Path,       // 경로(통로)
        Spawn,      // 적 스폰 지점
        Goal,       // 적 목표 지점
        Blocked,    // 그 밖의 배치 불가
    }
    
    // 그리드의 한칸 상태.
    public class Tile
    {
        public Vector2Int Coord { get; }
        public TileKind Kind;
        public Unit Unit;

        public Tile(Vector2Int coord, TileKind kind)
        {
            Coord = coord;
            Kind = kind;
            Unit = null;
        }
        
        public bool IsBuildable()
        {
            //타일의 종류가 Buildable이면서 점유한 유닛이 없어야 Build 가능한 상태
            return (Kind == TileKind.Buildable) && (Unit == null);
        }
    }
}

