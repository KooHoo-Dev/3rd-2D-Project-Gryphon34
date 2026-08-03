using Jay;

namespace Study.MiniDefence
{
    // 현재 우리가 만들고 있는 디펜스게임은 몇몇 큰 흐름이 있습니다
    // 1. Ready : 유닛 구매, 판매 기능등이 활성화 됩니다.
    //            게임을 시작하거나, 웨이브를 클리어하면 진입
    // 2. InWave : 웨이브를 진행하는 단계.
    //             Ready상태에서 버튼을 눌러서 진입할 수 있습니다
    // 3. Defeat : 체력이 다 없어져서 패배하는 단계 입니다.
    // 4. Victory : 3웨이브를 클리어하면 진입하는. 승리 단계
    
    public class GameManager : SingletonBase<GameManager>
    {
        
    }
}