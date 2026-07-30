using System;
using UnityEngine;
using System.Collections.Generic;
using Study.Utilities;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Study.MiniDefence
{
    // 기본 사각형타일 맵을 "조립"하고, 마우스로 칠하고, 프리펩으로 변환하는
    // 에디터용 도구. 런타임에는 사용되지 않습니다.
    // SquareGrid와 Tile을 이용하여 게임 사용될 Map을 제작 할 수 있는
    // 툴 객체 입니다.
    
    public class MapBuilder : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private int width = 10;
        [SerializeField] private int height = 8;
        [SerializeField] private float cellSize = 1f;
        
        [Header("Tile Visual Settings")]
        [SerializeField] private Sprite squareSprite;   // 타일 스프라이트. 비우면 흰 사각형을 에셋으로 자동 생성(색은 틴트)
        [SerializeField, Range(0f, 0.9f)] private float padding = 0f;   // 타일 사이 시각적 간격. 좌표/로직엔 영향 없음
        [SerializeField] private Color buildableColor = new Color(0.24f, 0.27f, 0.32f);
        [SerializeField] private Color pathColor = new Color(0.95f, 0.55f, 0.15f);   // 주황
        [SerializeField] private Color spawnColor = new Color(0.20f, 0.70f, 0.30f);  // 초록
        [SerializeField] private Color goalColor = new Color(0.90f, 0.30f, 0.30f);   // 빨강
        [SerializeField] private Color waypointColor = new Color(0.95f, 0.85f, 0.20f); // 노랑
        [SerializeField] private Color blockedColor = new Color(0.12f, 0.12f, 0.14f);
        
        [Header("Spawn & Goal Tile")]
        [SerializeField] private Vector2Int spawnTile = new Vector2Int(0, 4);
        [SerializeField] private Vector2Int goalTile = new Vector2Int(9, 4);
        
        [Header("Painting Settings")]
        [SerializeField] private bool enablePainting = true;
        [SerializeField] private TileKind paintKind = TileKind.Path;   // Game 화면에서 칠할 종류
        [SerializeField] private Camera paintCamera;                    // 비우면 Camera.main
        
        [Header("Save Path")]
        [SerializeField] private string prefabPath =
            "Assets/10. Scripts/1. Unity/15. Mini Defence/GeneratedMap.prefab";

        private GameObject mapRoot;
        private const string MAP_ROOT_NAME = "GeneratedMap";

        // 칸 상태는 칸마다 컴포넌트를 붙이는게 아니라 배열로 따로 관리한다.
        private TileKind[,] kinds;
        
        private MapData mapData;

        private SquareGrid grid;
        private SpriteRenderer[,] renderers;

        private void Start() => GenerateMap();

        #region Painting

        private void Update()
        {
            // 그리드가 생성되지 않았다면 페인팅 기능을 이용하지 못하게 한다.
            if (grid == null) return;

            bool left = SimpleInput.GetMouseButton(SimpleInput.MouseButton.Left);
            bool right = SimpleInput.GetMouseButton(SimpleInput.MouseButton.Right);

            if (left == false && right == false) return;

            // 스크린 좌표 가지고 와서
            Vector2 screen = SimpleInput.GetMousePosition();
            // 씬의 월드 좌표로 변환
            Vector3 world = paintCamera.ScreenToWorldPoint(new Vector3(screen.x, screen.y, 0));
            // Grid좌표로 변환
            Vector2Int coord = grid.WorldToCell(world);
            
            // 올바른 coord좌표를 얻었다면
            if (grid.InBounds(coord))
            {
                // 색을 칠한다(정보 변경)
                Paint(coord, left ? paintKind : TileKind.Buildable);
                // 색을 칠하고 맵 데이터에 반영해준다.
                // (최적화를 위해 원래는 왼쪽 클릭 해제 Or 오른쪽 클릭 해제시 동작하게 해줘야함.) 
                SyncMapData();  
            }
        }

        private void Paint(Vector2Int coord, TileKind kind)
        {
            // 이 함수는 시작할때부터 무조건 유효한 값을 가지고 시작합니다.
            // 따라서 grid.InBounds를 검사할 필요가 없다.
            // 하지만 넣고싶으면 넣으셔도 무방(아래처럼)
            // if (grid.InBounds(coord) == false) return;
            
            Tile tile = grid.GetTileOrNull(coord);
            // 해당 타일의 kind가 매개변수 kind와 동일하다면 return;
            if (tile.Kind == kind) return;
            
            // 스폰/목표 타일은 맵에 하나만 존재하기 때문에
            // 색칠 변경이 일어날 경우 이전에 있었던 타일을 BuildAble로 바꿔준다
            if (kind == TileKind.Spawn)
            {
                // 현재 spawnTile의 종류가 TileKind.Spawn인지 검사하고 
                if (IsKindAt(spawnTile, TileKind.Spawn))
                {
                    // 맞다면 바꿔준다
                    ApplyKind(grid.GetTileOrNull(spawnTile), TileKind.Buildable);
                }
                spawnTile = coord;
            }
            else if (kind == TileKind.Goal)
            {
                // 현재 spawnTile의 종류가 TileKind.Spawn인지 검사하고 
                if (IsKindAt(goalTile, TileKind.Goal))
                {
                    // 맞다면 바꿔준다
                    ApplyKind(grid.GetTileOrNull(goalTile), TileKind.Buildable);
                }
                
                goalTile = coord;
            }
            
            ApplyKind(tile, kind);
        }

        // 한 tile의 종류와 색을 결정한다.
        private void ApplyKind(Tile tile, TileKind kind)
        {
            tile.Kind = kind;
            renderers[tile.Coord.x, tile.Coord.y].color = ColorOf(tile.Kind);
        }

        #endregion
        
        # region Private Method
        
        private Vector3 CellToCenterLocal(Vector2Int c)
        {
            // 셀 중심좌표 위치 보정
            Vector3 reVal = new Vector3();
            reVal.x = (c.x + 0.5f) * cellSize;
            reVal.y = (c.y + 0.5f) * cellSize;
            return reVal;
        }

        private Color ColorOf(TileKind kind)
        {
            return kind switch
            {
                TileKind.Buildable => buildableColor,
                TileKind.Spawn => spawnColor,
                TileKind.Path => pathColor,
                TileKind.Goal => goalColor,
                TileKind.Blocked => blockedColor,
                _ => buildableColor
            };
        }
        
        TileKind KindOf(Vector2Int coord)
        {
            if (coord == spawnTile) return TileKind.Spawn;
            else if (coord == goalTile) return TileKind.Goal;
            return TileKind.Buildable;
        }
        
        private bool IsKindAt(Vector2Int coord, TileKind kind)
        {
            if (grid.InBounds(coord) == false) return false;
            return grid.GetTileOrNull(coord).Kind == kind;
        }
        
        # endregion
        
        # region MapData Method
        // WayPoint를 생성하는게 중요합니다

        // 현재 그리드에서 "정보만" 뽑아서 루트의 MapData에 기록한다.
        private void SyncMapData()
        {
            mapData.ImportFrom(grid, BuildRouteLocal());
        }

        private List<Vector3> BuildRouteLocal()
        {
            Vector2Int spawn = spawnTile;
            Vector2Int goal = goalTile;
            HashSet<Vector3> visitedPathTiles = new HashSet<Vector3>();
            
            List<Vector3> routePoints = new List<Vector3>();
            routePoints.Add(grid.CellToWorld(spawn));

            // --- 아래부터 본로직 ---
            
            // 검색 우선순위 순서대로 놓는다. 반대방향의 경우는 (i+2) % 4 로 구현
            Vector2Int[] dirs = new[] 
                { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };
            
            // usedRoad[x, y, dir] : 3차원 배열이고, 의미는 (x,y) 칸에서 dir 방향으로
            //                      길을 지나갔는가? 저장 용도
            bool[,,] usedRoad = new bool[grid.Width, grid.Height, dirs.Length];
            Vector2Int current = spawn; // 탐색 시작 인덱스를 지정해준다

            // 매번마다 Painting을 해서 방어코드가 하나 있어야 합니다(while 탈출 조건)
            // PS : 무한 루프 방지
            
            int guard = width * height * dirs.Length;
            while (true)
            {
                guard--;
                if (guard < 0) break;
                
                // --- 타일 검사 로직 --- 
                int chosen = -1; // 조건 검사에 의해 선택된 숫자
                for (int i = 0; i < dirs.Length; i++) // 4방향을 순서대로 검색을 해봅니다.
                {
                    // 이번에 조사해볼 타일의 인덱스를 고른다
                    Vector2Int node = current + dirs[i];  
                    // 맵의 범위 밖이면 Pass
                    if(grid.InBounds(node) == false) continue;
                    // Path 타입이 아니면 Pass
                    if(grid.GetTileOrNull(node).Kind != TileKind.Path) continue;
                    // 동일한 위치좌표에서 해당 방향으로 지나갔다면
                    if(usedRoad[current.x, current.y, i]) continue;
                    
                    chosen = i;
                    break;
                }

                if (chosen < 0) break; // 더 갈 길이 없으니 while문을 탈출
                
                // --- 경로 처리 로직 ---
                
                // 검사에 의해 선택된 방향을 좌표로 바꿔준다
                Vector2Int next = current + dirs[chosen]; //
                
                // 양쪽 모두 "지나간 길"로 표시한다
                usedRoad[current.x, current.y, chosen] = true;
                usedRoad[next.x, next.y, (chosen + 2) % dirs.Length] = true;

                // WayPoint를 남긴다
                routePoints.Add(grid.CellToWorld(next));
                current = next;
            }

            routePoints.Add(grid.CellToWorld(goal));
            return routePoints;
        }
        
        # endregion
        
        # region Context Menu Method

        [ContextMenu("Generate Map")]
        public void GenerateMap()
        {
            mapRoot = new GameObject(MAP_ROOT_NAME);
            mapRoot.transform.position = Vector3.zero;

            kinds = new TileKind[width, height];
            renderers = new SpriteRenderer[width, height];
            
            // 칸마다 종류를 정하고 그림(타일 오브젝트)을 만든다.
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2Int index = new Vector2Int(x, y);
                    CreateTile(index, KindOf(index)); // 스폰, 목표타일이 설정되어 넘어간다
                }
            }

            // 다 그린 뒤에 마지막으로 그리드를 완성(생성)한다.
            grid = SquareGrid.FromKinds(mapRoot.transform.position, cellSize, kinds);
            
            // 루트에 맵 정보(MapData)를 붙이고, 완성된 그리드에서 정보를 옮긴다.
            mapData = mapRoot.AddComponent<MapData>();
            SyncMapData();
            

            // 타일 자체를 만들지는 않음
            void CreateTile(Vector2Int coord, TileKind kind)
            {
                GameObject go = new GameObject($"Tile_{coord.x}_{coord.y}");
                go.transform.SetParent(mapRoot.transform);
                go.transform.localPosition = CellToCenterLocal(coord); 
                    
                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = squareSprite;
                sr.color = ColorOf(kind);
                
                kinds[coord.x, coord.y] = kind;
                renderers[coord.x, coord.y] = sr;
            }
        }

        [ContextMenu("Clear Map")]
        public void ClearMap()
        {
            if (mapRoot != null)
            {
                DestroyImmediate(mapRoot);
            }
        }

        [ContextMenu("Save Map As Prefab")]
        public void SaveMapAsPrefab()
        {
            // 전처리기 지시문?
            // : #if, #elif, #else, #endif 등의 키워드로 구성된 문법으로
            //  코드가 컴파일 되기전에 특정 조건에 따라 스크립트의 포함 여부를
            //  결정하는 기능. 주로 플랫폼(모바일, PC 등)에 맞춰서 코드를 분기
            //  해야할 떄 사용함. 여기서는 유니티 에디터 환경과 실제 빌드 환경의
            //  로직을 다르게 처리하기 위해 사용함
            //  
            //  주요 전처리기 지시문 키워드
            //  - UNITY_EDITOR
            //      : 유니티 에디터 환경에서 실행될 때만 포함되는 코드를 의미 
            //  - UNITY_STANDALONE_WIN / UNITY_STANDALONE_OSX
            //      : 윈도우나 맥환경등의 일반 PC
            //  - UNITY_ANDROID / UNITY_IOS
            //      : 모바일 디바이스(안드로이드, IOS)
            //  - UNITY_WEBGL
            //      : 웹 브라우저 플랫폼 빌드시 사용
            
#if UNITY_EDITOR

            if (mapRoot == null)
            {
                Debug.LogError($"[MapBuilder] 먼저 맵을 생성하세요!");
                return;
            }

            // PrefabUtility.SaveAsPrefabAsset ?
            // : 매개변수로 주어진 게임오브젝트(하위포함)를 지정된 경로에 프리펩 파일로 저장합니다.
            // 컴포넌트를 비롯한 모든 내용들이 저장됩니다. 프리펩 저장후 해당 프리펩의 객체를 반환합니다.
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset
                (mapRoot, prefabPath, out bool success);

            if (success && (prefab != null))
            {
                AssetDatabase.Refresh();
                Debug.Log($"[MapBuilder] 프리펩 저장 완료 : {prefabPath}");
            }
            else
            {
                Debug.LogError($"[MapBuilder] 프리펩 저장 실패 : {prefabPath}");
            }
#endif
        }
        
        # endregion
    }
}