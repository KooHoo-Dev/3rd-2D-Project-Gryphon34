
using System.Collections.Generic;
using System.IO;
using Jay.FileIO;
using UnityEngine;

namespace Study.MiniDefence
{
    public class UnitTable
    {
        private const string TableName = "Table/MiniDefence/units.tsv";
        
        private Dictionary<int, UnitDef> unitDefs = new();

        public int Count => unitDefs.Count;
        
        // 현재 테이블에 등록된 모든 Values(Unit들)을 읽기전용 콜렉션 형태로 리턴한다
        // 외부에서 foreach같은걸로 조회할수 있다
        public IReadOnlyCollection<UnitDef> Units => unitDefs.Values;
        public UnitDef Get(int id) => unitDefs[id];
        
        public void Load()
        {
            unitDefs.Clear();
            // DTO 읽고, Def로 바꿔서 저장해준다
            string path = Path.Combine(Application.streamingAssetsPath, TableName);
            List<UnitDTO> rows = TSVReader.ReadTable<UnitDTO>(path);

            for (int i = 0; i < rows.Count; i++)
            {
                UnitDTO dto = rows[i];
                Unit prefab = Resources.Load<Unit>(dto.PrefabKey);
                if(prefab == null) 
                    Debug.LogWarning($"[UnitTable] {dto.PrefabKey} 경로에 파일 없음");
                Sprite sprite = Resources.Load<Sprite>(dto.SpriteKey);
                if(sprite == null) 
                    Debug.LogWarning($"[UnitTable] {dto.SpriteKey} 경로에 파일 없음");

                UnitDef def = new UnitDef();
                def.ID = dto.ID;
                def.Name = dto.Name;
                def.Prefab = prefab;
                def.Sprite = sprite;
                def.Kind = dto.Kind;
                def.Targeting = dto.Targeting;
                
                def.Cost = dto.Cost;
                def.Damage = dto.Damage;
                def.Range = dto.Range;
                def.AttackInterval = dto.AttackInterval;
                def.CritChance = dto.CritChance;
                
                def.SplashRadius = dto.SplashRadius;
                def.MaxTargets = dto.MaxTargets;
                
                def.BuffRange = dto.BuffRange;
                def.BuffAtkPercent = dto.BuffAtkPercent;
                def.BuffSpeedPercent = dto.BuffSpeedPercent;
                
                // 하나하 매핑하서 def를 만들고 최종적으로
                // 딕셔너리에 넣어준다
                unitDefs.Add(dto.ID, def);
            }
        }
    }
}