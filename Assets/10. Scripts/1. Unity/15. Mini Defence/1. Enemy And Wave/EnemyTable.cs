using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Study.MiniDefence
{
    // "적 데이터의 로드 / 키 해석 / 보관 / 조회"의 책임을 갖는 객체
    // 새로운 데이터 종류가 생기면 이 클래스를 참고해서 만들면 됩니다.
    public class EnemyTable
    {
        private const string TableName = "Table/MiniDefence/enemies.tsv";

        private Dictionary<int, EnemyDef> defs = new();

        public EnemyDef Get(int id) => defs[id];

        public IReadOnlyCollection<EnemyDef> GetDefs() => defs.Values;
        
        public int Count => defs.Count;
        
        public void Load()
        {
            defs.Clear();
            
            // 특정 경로에 있는 파일을 읽어온다    
            string path = Application.streamingAssetsPath + "/" + TableName;
            List<EnemyDTO> rows = new List<EnemyDTO>();
            using (StreamReader reader = new StreamReader(path))
            {
                // 첫째줄(헤더)의 타입정의는 건너뛴다 (아래 내용)
                // ID	PrefabKey	SpriteKey	MaxHp	Speed	Armor	Resist	KillReward	LeakDamage
                reader.ReadLine();
                while (reader.EndOfStream == false)
                {
                    EnemyDTO enemyDto = new();
                    string line = reader.ReadLine(); // 한줄을 통으로 읽고
                    // line : 0	Mini Defence/Prefabs/Enemy/Enemy_00	Mini Defence/Prefabs/Enemy/Enemy_00_Thumbnail	100	2	0	0	10	1
                    string[] cells = line.Split('\t'); //.Split 함수를 이용해서 탭으로 잘라준다
                    //cells[0] : 0
                    //cells[1] : Mini Defence/Prefabs/Enemy/Enemy_00
                    //cells[2] : Mini Defence/Prefabs/Enemy/Enemy_00_Thumbnail
                    //cells[3] : 100
                    //cells[4] : 2
                    //cells[5] : 0
                    //cells[6] : 0
                    //cells[7] : 10
                    //cells[8] : 1
                    
                    // 차례대로 매핑한다
                    enemyDto.ID = int.Parse(cells[0]);
                    enemyDto.PrefabKey = cells[1];
                    enemyDto.SpriteKey = cells[2];
                    enemyDto.MaxHp = int.Parse(cells[3]);
                    enemyDto.Speed = float.Parse(cells[4]);
                    enemyDto.Armor = int.Parse(cells[5]);
                    enemyDto.Resist = float.Parse(cells[6]);
                    enemyDto.KillReward  = int.Parse(cells[7]);
                    enemyDto.LeakDamage = int.Parse(cells[8]);
                    
                    rows.Add(enemyDto);
                }
            }

            // List<EnemyDTO> rows 를 이용해서 def 객체를 생성한다
            foreach (EnemyDTO enemyDto in rows)
            {
                Enemy prefab = Resources.Load<Enemy>(enemyDto.PrefabKey);
                Sprite sprite = Resources.Load<Sprite>(enemyDto.SpriteKey);

                defs[enemyDto.ID] = new EnemyDef
                {
                    ID = enemyDto.ID,
                    Prefab = prefab,
                    Sprite = sprite,
                    Armor = enemyDto.Armor,
                    KillReward = enemyDto.KillReward,
                    LeakDamage = enemyDto.LeakDamage,
                    MaxHp = enemyDto.MaxHp,
                    Resist = enemyDto.Resist,
                    Speed = enemyDto.Speed,
                };
            }

            Debug.Log(defs.Count);
        }

    }
}