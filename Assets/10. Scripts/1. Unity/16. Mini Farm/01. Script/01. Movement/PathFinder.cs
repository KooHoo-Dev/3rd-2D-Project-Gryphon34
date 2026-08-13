using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Study.MiniFarm
{
    public class PathFinder : MonoBehaviour
    {
        // 나중에 지울거
        public MapData MapData => mapData;
        
        // 4방향 계산만 합니다. 8방향은 Astar With PriorityQueue 에서 보세여
        private static readonly Vector2Int[] Directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,
        };

        [Header("Settings")] 
        [SerializeField] private MapData mapData;
        [SerializeField] private MapLoader mapLoader;
        
        public Transform goalMarker;
        public Transform startMarker;
        
        public FarmGrid Grid { get; private set; }

        public bool IsReady { get; private set; } = false;
        
        private async void Start()
        {
            await DataManager.Instance.EnsureLoadedAsync();
            mapData = mapLoader.LoadOrNull();
            Grid = mapData.ToGrid(DataManager.Instance.Tiles);
            Debug.Log($"[PathFinder] 준비 완료. {Grid.Width}, {Grid.Height}");
            IsReady = true;
        }

        private void Update()
        {
            if (SimpleInput.GetKeyDown(Key.Space))
            {
                Tile start = Grid.GetTileAtWorldOrNull(startMarker.position);
                Tile goal = Grid.GetTileAtWorldOrNull(goalMarker.position);
                
                FindWaypointsOrNull(start.Coord, goal.Coord);
            }
        }

        # region Public Method
        
        public Vector2Int WorldToCoord(Vector3 world) => Grid.WorldToCell(world);
        public Vector3 CoordToWorld(Vector2Int coord) => Grid.CellToWorld(coord);
        public List<Vector3> FindWaypointsOrNull(Vector3  start, Vector3 goal)
            =>  FindWaypointsOrNull(WorldToCoord(start), WorldToCoord(goal));
        
        public List<Vector3> FindWaypointsOrNull(Vector2Int startCoord, Vector2Int goalCoord)
        {
            Tile start = Grid.GetTileOrNull(startCoord);
            Tile goal = Grid.GetTileOrNull(goalCoord);

            // 시작이 맵 밖이면 부르는 쪽의 배치가 잘못된 것이다. 조용히 넘기지 않는다.
            if (start == null)
            {
                Debug.LogError($"[PathFinder] 시작 좌표 {startCoord} 가 맵 밖입니다.");
                return null;
            }

            // 맵 밖을 찍는 것은 흔한 일이다. 오류가 아니므로 조용히 돌려보낸다.
            if (goal == null)
            {
                ClearGizmoPath();
                return null;
            }

            List<Tile> path = FindPathOrNull(start, goal);
            
            if (path == null)
            {
                ClearGizmoPath();
                return null;
            }
            
            return ToWaypoints(start, path);
        }
        
        #endregion
        
        # region 경로 찾기 함수들
        
        // openList, closedList 아님. 마지막으로 검색한 경로 보여주기 용도
        private readonly List<Tile> visited = new List<Tile>();
        
        // A* 본체. 시작 칸에서 목표 칸까지 밟고 갈 타일들을 돌려준다.
        // (시작 칸 빼고)
        private List<Tile> FindPathOrNull(Tile start, Tile goal)
        {
            if (goal.IsWalkable == false) return null;
            
            // Astar 시작하기전에 이전 계산때 사용했던 내용들을 초기화 해준다
            Grid.ResetPathCosts();
            visited.Clear();
            
            // 오픈리스트, 클로즈드 리스트 준비
            List<Tile> openList = new List<Tile>();
            openList.Add(start);
            HashSet<Tile> closedList = new HashSet<Tile>();
            
            // 시작타일의 G,H 값을 세팅해줍니다
            start.GCost = 0;
            start.HCost = Heuristic(start, goal);

            while (openList.Count > 0)
            {
                // openList에서 FCost가 가장 낮은 타일을 골라준다.
                Tile current = GetLowestFCostTile(openList);
                // 현재 타일이 goal 타일이면
                if (current.Equals(goal))
                {
                    // 경로를 반환하고 함수를 종료시킨다.
                    return RetracePath(start, goal);
                }
                
                // 타일 보관 관련 처리
                openList.Remove(current);
                closedList.Add(current);
                visited.Add(current);

                for (int i = 0; i < Directions.Length; i++)
                {
                    // 내 위치에 Directions[i]를 더해서 이웃 타일을 가져온다
                    Tile neighbour = Grid.GetTileOrNull(current.Coord + Directions[i]);
                    if(neighbour == null) continue;
                    if(neighbour.IsWalkable == false) continue;
                    if(closedList.Contains(neighbour)) continue;
                    
                    // G를 구해줍니다
                    // G : 현재까지 지나온 비용(current.GCost + neighbour.MoveCost)가 됩니다
                    // (H를 계산하는것과 다릅니다)

                    int tentativeG = current.GCost + neighbour.MoveCost;
                    
                    // neighbour의 G를 갱신합니다. 다만, 더 싼길을 찾았을 때만 갱신 합니다. 
                    if (tentativeG < neighbour.GCost)
                    {
                        neighbour.Parent = current;
                        neighbour.GCost = tentativeG;
                        neighbour.HCost = Heuristic(neighbour, goal);
                        
                        // openList에 neighbour가 있으면 위의 갱신만 하면 됨.
                        // 없다면 추가해준다
                        if(openList.Contains(neighbour) == false)
                            openList.Add(neighbour);
                    }
                }
            }
            
            return null;
        }

        private int MinMoveCost => DataManager.Instance.Tiles.MinMoveCost;
        
        // 목표 까지 남은 비용의 추측값
        // 지형정보들을 무시하고 순수 예측을 위한 값입니다.
        private int Heuristic(Tile from, Tile to)
        {
            // 타일의 디스턴스로 간단하게 예측 가능합니다.
            
            int distanceX = Mathf.Abs(from.Coord.x - to.Coord.x);
            int distanceY = Mathf.Abs(from.Coord.y - to.Coord.y);

            // Astar 내부에서 사용하는 값이니, 실제 길이와 다르더라도
            // 비교만 가능하면 ㅇㅋ. (4방향 직선이라서 기냥 더해도 되네)
            return (distanceX + distanceY) * MinMoveCost;
        }

        // openList에서 F가 가장 작은 칸을 고른다.
        // F가 같으면 H가 작은 쪽, 즉 목표에 더 가까운 쪽을 먼저 본다.
        private Tile GetLowestFCostTile(List<Tile> openList)
        {
            Tile lowest = openList[0];

            for (int i = 1; i < openList.Count; i++)
            {
                Tile tile = openList[i];
                if(tile.FCost > lowest.FCost) continue;

                if (tile.FCost == lowest.FCost)
                {
                    if (tile.HCost < lowest.HCost) lowest = tile;
                }
                else lowest = tile;
            }
            
            return lowest;
        }

        // 목표에서 Parent를 타고 거꾸로 올라가며 경로를 List에 넣어서
        // 반환해 준다. 완성된 경로는 목표에서부터 되짚어야 나온다.
        private List<Tile> RetracePath(Tile start, Tile goal)
        {
            List<Tile> path = new List<Tile>();
            
            // 탐색할 빈 타일 객체에 goal을 넣어준다
            Tile current = null;
            current = goal;

            while (current != start)
            {
                path.Add(current);
                current = current.Parent;
            }
            
            // 목표 -> 출발 순으로 쌓여있으므로 리스트를 뒤집어서 반환한다.
            // 출발칸은 안넣습니다.
            path.Reverse();
            
            return path;
        }
        
        #endregion
        
        #region 씬 뷰 표시

        private readonly List<Vector3> lastWaypoints = new();
        private Vector3 lastStart;
        private bool hasLastPath;
        public bool drawVisited = true;
        public bool drawPath = true;

        private List<Vector3> ToWaypoints(Tile start, List<Tile> path)
        {
            lastStart = Grid.CellToWorld(start.Coord);
            lastWaypoints.Clear();

            foreach (Tile tile in path)
            {
                lastWaypoints.Add(Grid.CellToWorld(tile.Coord));
            }

            hasLastPath = true;

            // 바깥에 넘길 목록은 따로 만든다.
            // 안쪽 목록을 그대로 주면 다음 탐색 때 지워져서 쓰던 쪽이 망가진다.
            return new List<Vector3>(lastWaypoints);
        }

        private void ClearGizmoPath()
        {
            lastWaypoints.Clear();
            hasLastPath = false;
        }

        // 가장 최근에 계산한 경로를 점과 선으로 그린다.
        private void OnDrawGizmos()
        {
            if (Grid == null) return;

            if (drawVisited) DrawVisited();
            if (drawPath) DrawLastPath();
        }

        private void DrawVisited()
        {
            Gizmos.color = new Color(0.3f, 0.6f, 1f, 0.30f);

            foreach (Tile tile in visited)
            {
                Gizmos.DrawCube(Grid.CellToWorld(tile.Coord), Vector3.one * Grid.CellSize * 0.45f);
            }
        }

        private void DrawLastPath()
        {
            if (hasLastPath == false || lastWaypoints.Count == 0) return;

            float radius = Grid.CellSize * 0.12f;

            // 출발점
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(lastStart, radius * 1.5f);

            // 지나갈 지점들과 그 사이를 잇는 선
            Gizmos.color = Color.yellow;
            Vector3 previous = lastStart;

            foreach (Vector3 point in lastWaypoints)
            {
                Gizmos.DrawLine(previous, point);
                Gizmos.DrawSphere(point, radius);
                previous = point;
            }

            // 도착점
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(lastWaypoints[^1], radius * 2f);
        }

        #endregion
    }
}