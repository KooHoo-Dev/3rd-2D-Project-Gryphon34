using System.Collections.Generic;
using System.IO;
using Jay.FileIO;
using UnityEngine;

namespace Study.MiniDefence
{
    // WaveTable은 EnemyDef가 필요하기 때문에
    // 생성시 EnemyTable을 주입반드다.
    public class WaveTable
    {
        private const string TableName = "Table/MiniDefence/waves.tsv";
        
        private Dictionary<int, WaveDef> waveDefs = new();
        
        public int Count => waveDefs.Count;
        public WaveDef Get(int i) => waveDefs[i];

        public void Load(EnemyTable enemyTable)
        {
            waveDefs.Clear();
            // Path클래스의 Combine함수를 이용해서 경로를 이어준다.
            string filePath = Path.Combine(Application.streamingAssetsPath, TableName);
            // 전달드린 TSVReader로 해당 테이블과 DTO 클래스 타입을 넘기면
            // 파싱 쌀먹을 할 수 있습니다. 유용히 쓰십쇼
            List<WaveDTO> rows = TSVReader.ReadTable<WaveDTO>(filePath);
            foreach (WaveDTO dto in rows)
            {
                EnemyDef enemyDef = enemyTable.Get(dto.EnemyID);

                waveDefs[dto.ID] = new WaveDef()
                {
                    ID = dto.ID,
                    Enemy = enemyDef,
                    Count = dto.Count,
                    Delay =  dto.Delay,
                    Interval =  dto.Interval,
                    RewardID = dto.RewardID,
                };
            }
        }
    }
}