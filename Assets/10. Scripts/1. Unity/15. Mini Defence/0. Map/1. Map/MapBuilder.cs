using System;
using UnityEngine;
using System.Collections.Generic;
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
        private SpriteRenderer[,] renderers;
        private MapData mapData;
        

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
        
        # region MapData Method

        private void SyncMapData()
        {
            
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
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2Int index = new Vector2Int(x, y);
                    CreateTile(index, KindOf(index)); // 스폰, 목표타일이 설정되어 넘어간다
                }
            }

            mapData = mapRoot.AddComponent<MapData>();
            SyncMapData();
            

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

            TileKind KindOf(Vector2Int coord)
            {
                if (coord == spawnTile) return TileKind.Spawn;
                else if (coord == goalTile) return TileKind.Goal;
                return TileKind.Buildable;
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