using Study.Utilities;
using UnityEngine;

namespace Study.MiniDefence
{
    public class Study_PlacementSystem : MonoBehaviour
    {
        public MapData mapData;
        private SquareGrid grid;

        private void Start()
        {
            grid = mapData.ToGrid();
        }
        
        private void Update()
        {
            if (SimpleInput.GetMouseButtonDown(0))
            {
                Tile spawnTile = BuildableTile();
                if(spawnTile != null) SpawnOne(spawnTile);
            }
        }

        private Tile BuildableTile()
        {
            Vector2 mousePosition = SimpleInput.GetMousePosition();
            Vector3 worldMousePoint = Camera.main.ScreenToWorldPoint(mousePosition);
            worldMousePoint.z = transform.position.z;
            Debug.Log(worldMousePoint);
            Vector2Int tilePos = grid.WorldToCell(worldMousePoint);
            Debug.Log(tilePos);
            Tile tile = grid.GetTileOrNull(tilePos);
            
            if (tile ==null || tile.Kind == TileKind.Blocked)
            {
                return null;
            }
            return tile;
        }

        private void SpawnOne(Tile tile)
        {
            Vector2 spawnPosition = tile.Coord;
            int randNum = Random.Range(0, 3);
            switch (randNum)
            {
                case 0: Unit unit0 = Resources.Load<Unit>("Assets/Resources/Mini Defence/Prefabs/Circle.prefab");
                    Instantiate(unit0, new Vector3(spawnPosition.x, spawnPosition.y, 0), Quaternion.identity);
                    tile.Kind = TileKind.Blocked;
                    break;
                case 1: Unit unit1 = Resources.Load<Unit>("Assets/Resources/Mini Defence/Prefabs/Square.prefab");
                    Instantiate(unit1, new Vector3(spawnPosition.x, spawnPosition.y, 0), Quaternion.identity);
                    tile.Kind = TileKind.Blocked;
                    break;
                case 2: Unit unit2 = Resources.Load<Unit>("Assets/Resources/Mini Defence/Prefabs/Triangle.prefab");
                    Instantiate(unit2, new Vector3(spawnPosition.x, spawnPosition.y, 0), Quaternion.identity);
                    tile.Kind = TileKind.Blocked;
                    break;
            }
            
        }
        
    }
}