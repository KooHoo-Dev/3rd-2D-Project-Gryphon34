using System.Collections.Generic;
using System.IO;
using Jay.FileIO.JsonDataModule;
using Study.MiniDefence._3._Combat;
using UnityEngine;

namespace Study.MiniDefence
{
    public struct UnitSaveData
    {
        public int DefID;
        public int X;
        public int Y;
    }
    
    // 진행 상황을 파일에 담고 되돌리는 담당
    // ! 주의 !
    // 설정(GameSetting, config)와 저장되는 경로가 다릅니다.
    // - 설정은 : StreamingAssets. 게임의 규칙, 읽기만 한다
    // - 유저데이터 : persistentDataPath - 플레이어의 진행상황. 읽고 쓴다.
    public static class SaveService
    {
        private const string FILE_NAME = "minidefence_save.json";
        
        // 저장 파일 안에서 각 값이 자리잡는 이름표.
        // 저장 파일안에서는 파스칼케이스를 사용하지 않습니다.
        private const string KEY_WAVE = "wave";         // int 단일
        private const string KEY_MINERALS = "minerals"; // int 단일
        private const string KEY_LIVES = "lives";       // int 단일
        private const string KEY_UNITS = "units";       // 복잡구조 여러개

        private static string FilePath
            => Path.Combine(Application.persistentDataPath, FILE_NAME);
        
        // 해당 경로에 FilePath의 파일이 있는지 검증
        public static bool HasSave => File.Exists(FilePath);
        
        # region 저장

        public static void Save()
        {
            // JsonDataModule Save 사용법
            // 1. 파일을 객체를 생성한다
            // 2. 파일 객체를 빌더 패턴들 사용하면서 데이터를 기록한다
            // 3. 파일 객체에 저장할 주소(path)를 넘기면 된다
            // Tip : 3번을 하면서 동시에 로그를 찍으면 쉽다

            JsonDataFile file = new JsonDataFile();

            file.Register(new JsonDataModule<int>(KEY_WAVE, WaveManager.Instance.CurrentWaveNo))
                .Register(new JsonDataModule<int>(KEY_MINERALS, EconomyManager.Instance.Minerals))
                .Register(new JsonDataModule<int>(KEY_LIVES, GameManager.Instance.Lives))
                .Register(new ListDataModule<UnitSaveData>(KEY_UNITS, CollectUnits()));

            file.Save(FilePath);
            Debug.Log($"[SaveService] Saved : {FilePath}");
        }

        private static List<UnitSaveData> CollectUnits()
        {
            List<UnitSaveData> result = new List<UnitSaveData>();

            var all = UnitRegistry.All;

            foreach (Unit unit in all)
            {
                UnitSaveData data = new UnitSaveData();
                data.DefID = unit.Def.ID;
                data.X = unit.Coord.x;
                data.Y = unit.Coord.y;
                result.Add(data);
            }
            
            return result;
        }
        
        # endregion

        #region 불러오기

        public static void Load()
        {
            // JsonDataModule Load 사용법
            // 1. 파일을 객체를 생성한다
            // 2. 파일 객체를 빌더 패턴들 사용하면서 기본값을 넣어준다
            // 3. 파일 객체에 불러올 주소(path)를 넘기면 된다
            // Tip : 3번을 하면서 동시에 로그를 찍으면 쉽다
            
            JsonDataFile file = new JsonDataFile();

            var wave = new JsonDataModule<int>(KEY_WAVE, 0);
            var minerals = new JsonDataModule<int>(KEY_MINERALS, 0);
            var lives = new JsonDataModule<int>(KEY_LIVES, 0);
            var units = new ListDataModule<UnitSaveData>(KEY_UNITS, new List<UnitSaveData>());
            
            file.Register(wave)
                .Register(minerals)
                .Register(lives)
                .Register(units);
            
            file.Load(FilePath);
            Debug.Log($"[LoadService] Loaded : {FilePath}");
            
            // 게임을 진행하는 도중에 로드가 일어날 경우
            // 현재 보드를 비우는걸 먼저하고
            // 기록에 있는 유닛을 "배치"해줍니다
            RemoveAllUnits();
            RebuildUnits(units.Value);
            
            
            WaveManager.Instance.Restore(wave.Value);
            GameManager.Instance.GainLives(lives.Value);

            // 바로 대입하는게 아니라, 소득/지출 등의 기록을 남길 수 있도록
            // 함수를 통해서 Set해준다. 내가 전공이 회계여서 이런식으로 한다
            int diff = minerals.Value - EconomyManager.Instance.Minerals;
            if(diff > 0) EconomyManager.Instance.Earn(diff);
            else if (diff < 0) EconomyManager.Instance.TrySpend(-diff); 
            
        }

        private static void RemoveAllUnits()
        {
            // 특정 리스트의 개체를 지울때는 리스트 자체의 크기가 변경될수있음으로
            // 사본을 만들어서 삭제를 진행 해줍니다
            List<Vector2Int> coords = new List<Vector2Int>();

            foreach (var unit in UnitRegistry.All)
                coords.Add(unit.Coord);

            foreach (var coord in coords)
                PlacementSystem.Instance.Remove(coord);
        }

        private static void RebuildUnits(List<UnitSaveData> saved)
        {
            int totalCost = 0;

            // 총 비용먼저 계산 합니다.
            foreach (var item in saved)
                totalCost += DataManager.Instance.GetUnitDef(item.DefID).Cost;

            // EconomyManager에게 미리 돈을 줍니다
            EconomyManager.Instance.Earn(totalCost);

            foreach (var item in saved)
            {
                UnitDef def = DataManager.Instance.GetUnitDef(item.DefID);
                PlacementSystem.Instance.Build(def, new Vector2Int(item.X, item.Y));
            }
        }
        
        #endregion
    }
}