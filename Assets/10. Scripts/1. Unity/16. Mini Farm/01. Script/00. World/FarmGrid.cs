using System.Collections.Generic;
using UnityEngine;

namespace Study.MiniFarm
{
    public class FarmGrid
    {
        public int Width { get; }
        public int Height { get; }
        public float CellSize { get; }
        public Vector2 Origin { get; }

        private readonly Tile[,] tiles;

        public FarmGrid(int width, int height, float cellSize, Vector2 origin, TileDef defaultDef)
        {
            Width = width;
            Height = height;
            CellSize = cellSize;
            Origin = origin;

            tiles = new Tile[width, height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    tiles[x, y] = new Tile(new Vector2Int(x, y), defaultDef);
                }
            }
        }

        public static FarmGrid FromTypeList(
            int width, int height, float cellSize, Vector2 origin,
            List<TileType> types, TileTable table)
        {
            FarmGrid grid = new FarmGrid(width, height, cellSize, origin, table.Get(TileType.Soil));

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    TileType type = types[y * width + x];
                    grid.tiles[x, y].ChangeType(table.Get(type));
                }
            }

            return grid;
        }

        public List<TileType> ToTypeList()
        {
            List<TileType> types = new List<TileType>(Width * Height);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    types.Add(tiles[x, y].Type);
                }
            }

            return types;
        }

        public bool InBounds(int x, int y)
        {
            bool inBoundX = (0 <= x) && (x < Width);
            bool inBoundY = (0 <= y) && (y < Height);

            return inBoundX && inBoundY;
        }

        public bool InBounds(Vector2Int coord) => InBounds(coord.x, coord.y);

        public Tile GetTileOrNull(int x, int y)
        {
            if (InBounds(x, y) == false) return null;
            return tiles[x, y];
        }

        public Tile GetTileOrNull(Vector2Int coord) => GetTileOrNull(coord.x, coord.y);

        public Vector2Int WorldToCell(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt((worldPosition.x - Origin.x) / CellSize);
            int y = Mathf.FloorToInt((worldPosition.y - Origin.y) / CellSize);

            return new Vector2Int(x, y);
        }

        public Vector3 CellToWorld(Vector2Int coord)
        {
            float x = Origin.x + (coord.x + 0.5f) * CellSize;
            float y = Origin.y + (coord.y + 0.5f) * CellSize;

            return new Vector3(x, y, 0f);
        }

        public Tile GetTileAtWorldOrNull(Vector3 worldPosition)
            => GetTileOrNull(WorldToCell(worldPosition));

        public void ResetPathCosts()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    tiles[x, y].ResetPathCosts();
                }
            }
        }
    }
}
