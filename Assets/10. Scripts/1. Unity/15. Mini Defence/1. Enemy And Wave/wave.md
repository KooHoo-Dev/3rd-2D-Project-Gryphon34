```mermaid
classDiagram
    class WaveTester {
        -int PlayerHp
        -Update()
        -PlayerHpUpdate()
    }
    class WaveManager {
        -WaveDef currentWave
        -MapData currentMap
        +StartWave()
        -SpawnOne()
        -WaveCoroutine()
        -HandleGoalReached()
    }
    class Enemy {
        -Queue~Vector3~ moveQueue
        +Init()
        -MoveNext()
    }
    class WaveDef {
        +EnemyDef Enemy
        +int Count
        +float Interval
        +float Delay
    }
    class EnemyDef {
        +float Speed
        +int LeakDamage
    }

    WaveTester --> WaveManager : 1번 키 입력 시 시작 지시
    WaveManager --> WaveDef : 웨이브 진행에 필요한 정보 참조
    WaveManager --> Enemy : 프리팹 복제 및 초기화 지시
    WaveDef --> EnemyDef : 소환할 적의 기본 데이터 보관
    Enemy --> EnemyDef : 이동 속도 등 자신의 능력치 참조
```

```mermaid
sequenceDiagram
    participant WT as WaveTester
    participant WM as WaveManager
    participant E as Enemy

    Note over WT, WM: [1. 웨이브 시작 단계]
    WT->>WM: StartWave() 호출 (키보드 1번 입력)
    WM->>WM: WaveCoroutine() 코루틴 시작
    Note over WM: WaveDef.Delay 시간만큼 최초 대기

    Note over WM, E: [2. 적 생성 단계]
    loop WaveDef.Count 횟수만큼 반복
        WM->>E: Instantiate()로 빈 오브젝트 생성
        WM->>E: Init() 호출 (목표 경로 리스트 전달)
        E->>E: 경로를 큐(Queue)에 넣고 첫 위치로 순간 이동
        WM->>E: GoalReached 이벤트 구독 (도착 시 알림 받기 위함)
        Note over WM: WaveDef.Interval 시간만큼 대기 후 다음 적 생성
    end

    Note over E: [3. 적 이동 단계]
    E->>E: MoveNext() 호출
    Note over E: DOTween을 이용해 큐에 남은 다음 위치로 선형 이동(Linear)
    
    Note over E, WT: [4. 목표 도달 및 데미지 처리 단계]
    E->>WM: 큐가 비면 GoalReached 이벤트 발생
    E->>E: 스스로 비활성화 (SetActive false)
    WM->>WM: HandleGoalReached 실행 (적 구독 해제)
    WM->>WT: EnemyReachedGoal 이벤트 발생시켜 외부로 중계
    WT->>WT: PlayerHpUpdate 실행 (LeakDamage만큼 플레이어 체력 감소)
```