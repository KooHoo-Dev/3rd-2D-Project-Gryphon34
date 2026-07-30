using System.IO;
using System.Net.Mime;
using UnityEngine;

namespace Study.MiniDefence
{
    // 게임 플레이에서 사용하는 주요 데이터들과 규칙들을 모아 두는 설정 덩어리
    // MonoBehaviour로 할 필요가 없는 이유는
    // 1. 씬별로 달라질 이유가 없으니까
    // 2. static이나 싱글톤(C#)으로 만들어 두면 인스펙터에서 배치를 깜빡한다거나
    //   그럴 일이 없다.
    
    [System.Serializable]
    public class GameSettings
    {
        public int StartingMinerals = 1000;     // 시작 광물량
        public float RefundRate = 0.5f;         // 유닛 판매시 환불 금액
        public int StartingLives = 20;          // 시작시 목숨
        public float CriMultiplier = 2f;        // 크리티컬 피해 배율
        
        private static GameSettings instance;

        public static GameSettings Instance
        {
            get
            {
                if (instance == null) instance = Load();
                return instance;
            }
        }

        // 외부의 세팅파일을 불러와서 객체를 생성하는 코드 입니다.
        private static GameSettings Load()
        {
            return JsonUtility.FromJson<GameSettings>(File.ReadAllText(JsonPath));
        }
        
        public static string JsonPath=>Path.Combine(Application.streamingAssetsPath, "Table","MiniDefence", "config.json");
    }
}