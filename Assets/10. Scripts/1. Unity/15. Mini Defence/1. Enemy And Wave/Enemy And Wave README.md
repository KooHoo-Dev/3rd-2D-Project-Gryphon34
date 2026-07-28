

# WaveManager
- 싱글톤 클래스. WaveManger.Instance로 전역 접근 가능
  - `WaveManger.Instance.StartWave(waveDef, map);`
- 정상작동을 위해 `WaveDef`, `MapData`, `EnemyDef`가 필요함
  - 사용예시 : `WaveTester` 구성을 참조. `1.Dev Wave And Enemy 씬`을 참고할것
- `public void StartWave(WaveDef wave, MapData map);` 함수를
  이용해서 웨이브 시작 가능
- MapData에 정의된 RoutePoint를 기반으로 WorldPoistion 상에서
  Enemy가 진군 하게끔 구현되어있음
- `public event Action<Transform, EnemyDef> EnemyReachedGoal;`에
  함수를 구독하여 적개체가 MapData 목적지에 도착했을시 알림 통보를 받을 수 있음

## Public Field

```csharp
public class WaveManager
{
    // Enemy 개체가 목적지에 도착시 해당 개체의 위치정보와
    // Def(정의)를 델리게이트를 통해 통보
    public event Action<Transform, EnemyDef> EnemyReachedGoal;
}
```

## Public Method(API)
```csharp
public class WaveManager
{
    // 외부에서 WaveDef, MapData를 주입받아서 동작 시킬 수 있음.
    // 데이터 주입 없을 시 동작 불가
    public void StartWave(WaveDef wave, MapData map);
}
```