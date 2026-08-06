using System.Collections.Generic;
using System.Threading.Tasks;
using Jay.FileIO;   // TSVReader
using UnityEngine;

namespace Study.MiniFarm
{
    // 타일 종류 데이터 전담.
    public class TileTable
    {
        // TSVReader 는 StreamingAssets/Table/{TableName}.tsv 를 읽는다.
        private const string TableName = "MiniFarm/tiles";

        private readonly Dictionary<TileType, TileDef> defs = new();

        public int Count => defs.Count;

        public TileDef Get(TileType type)
        {
            if (defs.TryGetValue(type, out TileDef def)) return def;

            Debug.LogError($"[TileTable] '{type}' 이(가) {TableName}.tsv 에 없습니다.");
            return null;
        }

        // 길찾기 휴리스틱이 쓰는 값. 통과 가능한 타일 중 가장 싼 비용이다.
        // 이 값보다 큰 수를 휴리스틱에 쓰면 A* 가 최단 경로를 놓친다.
        public int MinMoveCost { get; private set; }

        public async Task LoadAsync()
        {
            defs.Clear();

            List<TileDef> rows = await TSVReader.ReadTableAsync<TileDef>(TableName, true);
            if (rows == null)
            {
                Debug.LogError($"[TileTable] '{TableName}.tsv' 로드 실패");
                return;
            }

            int minCost = int.MaxValue;

            foreach (TileDef def in rows)
            {
                defs[def.TileType] = def;

                if (def.Walkable && def.MoveCost < minCost) minCost = def.MoveCost;
                if (def.Walkable && def.MoveCost <= 0)
                    Debug.LogError($"[TileTable] '{def.TileType}' 의 MoveCost 가 {def.MoveCost} 입니다. 1 이상이어야 합니다.");
            }

            MinMoveCost = minCost;

            // enum 에는 있는데 표에 없는 종류를 미리 잡는다.
            foreach (TileType type in System.Enum.GetValues(typeof(TileType)))
            {
                if (defs.ContainsKey(type) == false)
                    Debug.LogError($"[TileTable] '{type}' 행이 {TableName}.tsv 에 없습니다.");
            }

            Debug.Log($"[TileTable] 로드 완료 — 타일 {defs.Count}종 · 최소 이동 비용 {MinMoveCost}");
        }
    }
}
