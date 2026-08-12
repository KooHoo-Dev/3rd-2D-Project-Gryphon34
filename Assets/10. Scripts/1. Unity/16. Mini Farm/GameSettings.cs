using System.IO;
using UnityEngine;

namespace Study.MiniFarm
{
    [System.Serializable]
    public class GameSettings
    {
        [Header("시간")]
        public int TotalDays = 14;             // 총 기간
        public float DayLengthSeconds = 120f;  // 하루 길이 (배속 x1 기준, 초)

        public float[] TimeScales = { 0f, 1f, 2f, 3f };

        [Header("경제")]
        public int StartGold = 100;
        public int GoalGold = 800;
        public int StorageSlots = 12;
        public int HandMaxCount = 3;

        private static GameSettings instance;

        public static GameSettings Instance
        {
            get
            {
                if (instance == null) instance = Load();
                return instance;
            }
        }

        public static string JsonPath
            => Path.Combine(Application.streamingAssetsPath, "Table", "MiniFarm", "config.json");

        private static GameSettings Load()
        {
            GameSettings loaded = JsonUtility.FromJson<GameSettings>(File.ReadAllText(JsonPath));
            loaded.Validate();

            return loaded;
        }

        #region 값 검사

        private static readonly GameSettings Defaults = new GameSettings();

        private void Validate()
        {
            TotalDays = AtLeast(nameof(TotalDays), TotalDays, 1, Defaults.TotalDays);
            StorageSlots = AtLeast(nameof(StorageSlots), StorageSlots, 1, Defaults.StorageSlots);
            HandMaxCount = AtLeast(nameof(HandMaxCount), HandMaxCount, 1, Defaults.HandMaxCount);
            StartGold = AtLeast(nameof(StartGold), StartGold, 0, Defaults.StartGold);
            GoalGold = AtLeast(nameof(GoalGold), GoalGold, 1, Defaults.GoalGold);

            DayLengthSeconds = AtLeast(nameof(DayLengthSeconds), DayLengthSeconds, 0.1f, Defaults.DayLengthSeconds);

            ValidateTimeScales();
        }

        private static int AtLeast(string name, int value, int min, int fallback)
        {
            if (value >= min) return value;

            Debug.LogError($"[GameSettings] {name} 이(가) {value} 입니다. {min} 이상이어야 합니다. 기본값 {fallback} 로 되돌립니다.");
            return fallback;
        }

        private static float AtLeast(string name, float value, float min, float fallback)
        {
            if (value >= min) return value;

            Debug.LogError($"[GameSettings] {name} 이(가) {value} 입니다. {min} 이상이어야 합니다. 기본값 {fallback} 로 되돌립니다.");
            return fallback;
        }

        private void ValidateTimeScales()
        {
            bool ok = TimeScales != null
                   && TimeScales.Length >= 2
                   && TimeScales[0] == 0f
                   && TimeScales[1] > 0f;

            if (ok) return;

            Debug.LogError("[GameSettings] TimeScales 는 0번이 0(정지), 1번이 0보다 큰 값이어야 합니다. " +
                           $"기본값 [{string.Join(", ", Defaults.TimeScales)}] 로 되돌립니다.");

            TimeScales = (float[])Defaults.TimeScales.Clone();
        }

        #endregion
    }
}
