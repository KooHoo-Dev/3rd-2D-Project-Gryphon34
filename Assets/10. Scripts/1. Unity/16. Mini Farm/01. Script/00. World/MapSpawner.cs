using UnityEngine;

namespace Study.MiniFarm
{
    // 맵 파일 하나로 씬에 실제 오브젝트를 세우는 일만 맡는다.
    public static class MapSpawner
    {
        public const string MapRootName = "GeneratedFarmMap";

        public static MapData Spawn(MapFile file, Sprite tileSprite, Vector3 origin, Transform parent = null)
        {
            GameObject root = new GameObject(MapRootName);
            root.transform.SetParent(parent);
            root.transform.position = origin;

            for (int y = 0; y < file.Height; y++)
            {
                for (int x = 0; x < file.Width; x++)
                {
                    TileType type = file.Tiles[y * file.Width + x];
                    CreateTile(root.transform, new Vector2Int(x, y), type, file.CellSize, tileSprite);
                }
            }

            MapData data = root.AddComponent<MapData>();
            data.ApplyFile(file);

            return data;
        }

        private static void CreateTile(Transform parent, Vector2Int coord, TileType type, float cellSize, Sprite sprite)
        {
            GameObject go = new GameObject($"Tile_{coord.x}_{coord.y}");
            go.transform.SetParent(parent);
            go.transform.localPosition = new Vector3((coord.x + 0.5f) * cellSize, (coord.y + 0.5f) * cellSize, 0f);

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = ColorOf(type);

            sr.sortingOrder = -100;
        }

        public static Color ColorOf(TileType type)
        {
            switch (type)
            {
                case TileType.Road: return new Color(0.85f, 0.78f, 0.60f);
                case TileType.Soil: return new Color(0.55f, 0.40f, 0.26f);
                case TileType.Tilled: return new Color(0.38f, 0.26f, 0.16f);
                case TileType.Water: return new Color(0.20f, 0.45f, 0.80f);
                case TileType.Rock: return new Color(0.45f, 0.45f, 0.48f);
                case TileType.Fence: return new Color(0.30f, 0.25f, 0.20f);
            }

            Debug.LogError($"[MapSpawner] '{type}' 의 색이 정해지지 않았습니다. ColorOf 를 보세요.");
            return Color.magenta;
        }
    }
}