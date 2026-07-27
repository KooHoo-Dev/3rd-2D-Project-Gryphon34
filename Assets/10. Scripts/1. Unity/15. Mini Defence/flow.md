```mermaid
classDiagram
    class MapData {
        +List~TileKind~ TileKinds
        +ImportFrom(SquareGrid grid)
        +ToGrid() SquareGrid
    }
    class SquareGrid {
        -Tile[,] tiles
        +ToKindList() List~TileKind~
        +FromKinds(TileKind[,]) SquareGrid
        +FromKindList(List~TileKind~) SquareGrid
    }
    class Tile {
        +Vector2Int Coord
        +TileKind Kind
        +Unit Unit
        +IsBuildable() bool
    }

    MapData --> SquareGrid : 데이터 추출 및 복원
    SquareGrid "1" *-- "다수" Tile : 2차원 배열로 포함
```

```mermaid
sequenceDiagram
    participant MB as MapBuilder<br>(에디터 툴)
    participant SG as SquareGrid<br>(자료구조)
    participant T as Tile<br>(기본 단위)
    participant MD as MapData<br>(정보 보관소)

    Note over MB, MD: 1. 에디터 환경 (맵 조립 및 저장)
    MB->>SG: FromKinds() 호출하여 그리드 생성
    SG->>T: 지정된 크기만큼 Tile 객체들 생성
    MB->>SG: 마우스 클릭 시 Paint()로 정보 변경 지시
    SG->>T: 특정 위치의 Tile 상태 변경
    MB->>MD: SyncMapData() 내부에서 ImportFrom() 호출
    MD->>SG: ToKindList() 호출하여 데이터 요청
    SG-->>MD: 2차원 데이터를 1차원 리스트로 변환하여 반환
    Note right of MD: 변환된 리스트를 TileKinds에 저장 (프리팹 저장 준비 완료)

    Note over MB, MD: 2. 런타임 환경 (인게임 맵 로드)
    MD->>MD: 게임 시작 시 씬에 배치된 상태로 활성화
    MD->>SG: ToGrid() 내부에서 FromKindList() 호출
    SG->>T: 1차원 리스트를 바탕으로 Tile 객체들을 2차원으로 재생성
    SG-->>MD: 복원 완료된 SquareGrid 반환
    Note right of SG: 인게임 시스템들이 복원된 SquareGrid를 활용하여 로직 수행
```