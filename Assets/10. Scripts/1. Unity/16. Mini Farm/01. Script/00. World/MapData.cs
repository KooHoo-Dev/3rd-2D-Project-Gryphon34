using System.Collections.Generic;
using UnityEngine;

namespace Study.MiniFarm
{
    public class MapData : MonoBehaviour
    {
        public int Width = 24;
        public int Height = 16;
        public float CellSize = 1f;

        public List<TileType> TileTypes = new List<TileType>();
        public List<MapObjectEntry> Objects = new List<MapObjectEntry>();

        public void ImportFrom(FarmGrid grid)
        {
            Width = grid.Width;
            Height = grid.Height;
            CellSize = grid.CellSize;
            TileTypes = grid.ToTypeList();
        }

        #region 맵 파일과 주고받기

        public MapFile ToFile(string mapName)
        {
            return new MapFile
            {
                Name = mapName,
                Width = Width,
                Height = Height,
                CellSize = CellSize,
                Tiles = new List<TileType>(TileTypes),
                Objects = new List<MapObjectEntry>(Objects),
            };
        }

        public void ApplyFile(MapFile file)
        {
            Width = file.Width;
            Height = file.Height;
            CellSize = file.CellSize;
            TileTypes = new List<TileType>(file.Tiles);
            Objects = new List<MapObjectEntry>(file.Objects);
        }

        #endregion

        #region 칸 위에 무언가 올리기

        public Transform GetTileTransformOrNull(Vector2Int coord)
        {
            if (coord.x < 0 || coord.x >= Width) return null;
            if (coord.y < 0 || coord.y >= Height) return null;

            int index = coord.y * Width + coord.x;
            if (index >= transform.childCount) return null;

            return transform.GetChild(index);
        }

        public List<MapObjectEntry> FindObjects(string kind)
        {
            List<MapObjectEntry> found = new List<MapObjectEntry>();

            foreach (MapObjectEntry entry in Objects)
            {
                if (entry.Kind == kind) found.Add(entry);
            }

            return found;
        }

        #endregion

        public FarmGrid ToGrid(TileTable table)
        {
            return FarmGrid.FromTypeList(Width, Height, CellSize, transform.position, TileTypes, table);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Vector3 origin = transform.position;
            Vector3 size = new Vector3(Width * CellSize, Height * CellSize, 0f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(origin + size * 0.5f, size);

            if (TileTypes.Count != Width * Height) return;

            Gizmos.color = new Color(0.9f, 0.2f, 0.2f, 0.35f);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    TileType type = TileTypes[y * Width + x];
                    if (IsBlockedKind(type) == false) continue;

                    Vector3 center = origin + new Vector3((x + 0.5f) * CellSize, (y + 0.5f) * CellSize, 0f);
                    Gizmos.DrawCube(center, Vector3.one * CellSize * 0.8f);
                }
            }
        }

        private static bool IsBlockedKind(TileType type)
        {
            return type == TileType.Water
                || type == TileType.Rock
                || type == TileType.Fence;
        }
#endif
    }
}
