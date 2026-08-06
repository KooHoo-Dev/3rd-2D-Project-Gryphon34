using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Study.MiniFarm
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TileType
    {
        Road,       // 길. 가장 빠르다
        Soil,       // 흙. 밭을 갈 수 있는 유일한 타일
        Tilled,     // 갈아둔 밭. 씨앗을 심을 수 있고, 밟고 지나가기는 느리다
        Water,      // 물. 통과 불가
        Rock,       // 바위. 통과 불가
        Fence,      // 울타리. 통과 불가
    }

    // DTO 와 Def 를 나누지 않은 이유는
    // 미니 디펜스 에서는 프리팹 "이름"(문자열)이 적혀 있고, 그것을 실제 에셋 참조로 바꾸는
    // 해석 단계가 필요함. 그래서 EnemyDTO(적힌 글자)와 EnemyDef(쓰는 값)가 갈라짐.
    // 하지만 현재 타일에는 해석할 것이 없다. 숫자와 참/거짓뿐이고, 표에 적힌 값이 곧 쓰는 값.
    // 나눠 봐야 같은 필드를 한 벌 더 만들고 베껴 담는 코드만 늘어남.
    // - 표에 적힌 값을 그대로 쓴다      -> 한 클래스
    // - 키를 실제 에셋·다른 표로 해석한다 -> DTO 와 Def 를 나눈다
    public class TileDef
    {
        public TileType TileType;
        public string Name;

        // 이 타일을 밟고 지나갈 때 드는 비용. 길찾기의 G 계산에 더해진다.
        public int MoveCost;

        public bool Walkable;
        public bool Tillable;
    }
}
