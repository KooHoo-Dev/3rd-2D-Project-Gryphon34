using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Study.MiniFarm
{
    public class MapBuilder : MonoBehaviour
    {
        [Header("새 맵 크기")]
        [SerializeField] private int width = 24;
        [SerializeField] private int height = 16;
        [SerializeField] private float cellSize = 1f;

        [Header("칸 그림")]
        [SerializeField] private Sprite tileSprite;

        [Header("칠하기")]
        [SerializeField] private TileType paintType = TileType.Road;
        [SerializeField] private Camera paintCamera;

        [Header("맵 파일")]
        // StreamingAssets/Maps/MiniFarm/{이 이름}.json
        [SerializeField] private string mapName = "Farm01";

        [Header("프리팹 저장 위치")]
        [SerializeField] private string prefabPath =
            "Assets/00. Study/16. Mini Farm/GeneratedFarmMap.prefab";

        private MapData mapData;
        private FarmGrid grid;

        private TileTable Tiles => DataManager.Instance.Tiles;

        private async void Start()
        {
            await DataManager.Instance.EnsureLoadedAsync();
            if (paintCamera == null) paintCamera = Camera.main;
        }

        #region 칠하기

        private void Update()
        {
            if (grid == null) return;

            bool left = Mouse.current.leftButton.isPressed;
            bool right = Mouse.current.rightButton.isPressed;

            if (left == false && right == false) return;

            Vector3 world = paintCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2Int coord = grid.WorldToCell(world);

            if (grid.InBounds(coord) == false) return;

            Paint(coord, left ? paintType : TileType.Soil);
        }

        private void Paint(Vector2Int coord, TileType type)
        {
            Tile tile = grid.GetTileOrNull(coord);
            if (tile.Type == type) return;

            tile.ChangeType(Tiles.Get(type));

            Transform tileObject = mapData.GetTileTransformOrNull(coord);
            tileObject.GetComponent<SpriteRenderer>().color = MapSpawner.ColorOf(type);
            
            mapData.ImportFrom(grid);
        }

        #endregion

        #region 맵 만들기 · 불러오기

        [ContextMenu("Generate Map")]
        public void GenerateMap()
        {
            if (IsTableReady() == false) return;
            
            MapFile empty = new MapFile
            {
                Name = mapName,
                Width = width,
                Height = height,
                CellSize = cellSize,
            };

            for (int i = 0; i < width * height; i++) empty.Tiles.Add(TileType.Soil);
            Build(empty);
        }

        [ContextMenu("Import From Json")]
        public void ImportFromJson()
        {
            if (IsTableReady() == false) return;

            MapFile file = MapFile.LoadOrNull(mapName);
            if (file == null) return;

            Build(file);

            Debug.Log($"[MapBuilder] '{mapName}' 을(를) 불러왔습니다 — {file.Width} x {file.Height}");
        }

        [ContextMenu("Export To Json")]
        public void ExportToJson()
        {
            if (mapData == null)
            {
                Debug.LogError("[MapBuilder] 먼저 맵을 만들거나 불러오세요.");
                return;
            }

            mapData.ToFile(mapName).Save(mapName);

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        [ContextMenu("Clear Map")]
        public void ClearMap()
        {
            if (mapData != null) DestroyImmediate(mapData.gameObject);

            mapData = null;
            grid = null;
        }

        private void Build(MapFile file)
        {
            ClearMap();

            mapData = MapSpawner.Spawn(file, tileSprite, transform.position, transform);
            grid = mapData.ToGrid(Tiles);
        }

        private bool IsTableReady()
        {
            if (Tiles.Count > 0) return true;

            Debug.LogError("[MapBuilder] 표를 아직 읽지 못했습니다. 플레이 모드에서 실행해 주세요.");
            return false;
        }

        #endregion

        #region 프리팹으로 저장

        [ContextMenu("Save As Prefab")]
        public void SaveMapAsPrefab()
        {
#if UNITY_EDITOR
            if (mapData == null)
            {
                Debug.LogError("[MapBuilder] 먼저 맵을 만들거나 불러오세요.");
                return;
            }

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(mapData.gameObject, prefabPath, out bool success);

            if (success && prefab != null)
            {
                AssetDatabase.Refresh();
                Debug.Log($"[MapBuilder] 프리팹 저장 완료 : {prefabPath}");
            }
            else
            {
                Debug.LogError($"[MapBuilder] 프리팹 저장 실패 : {prefabPath}");
            }
#endif
        }

        #endregion
    }
}
