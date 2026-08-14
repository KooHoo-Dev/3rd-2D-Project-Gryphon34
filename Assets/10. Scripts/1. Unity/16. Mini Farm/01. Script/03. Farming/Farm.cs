using System;
using System.Collections.Generic;
using UnityEngine;

namespace Study.MiniFarm
{
    // 복잡성이 높은 주요 클래스들을 개발할때는
    // 애초부터 로그를 찍을 생각으로 검증하는 하나의 단위를 만들면서 개발하는게
    // 속도가 더 빠릅니다
    
    // 농사 행동(4가지 행동 포함)을 시도한 결과
    // 를 데이터 정의를 해놓습니다.
    public enum FarmResult
    {
        Ok,
        WrongTile,          // 그 행동을 할 수 없는 종류의 칸 일 때
        AlreadyTilled,      // 이미 갈아둔 밭(타일)이다 : Till의 결과
        AlreadyPlanted,     // 이미 뭔가 심어져 있다    : Sow의 결과중 하나
        NoCrop,             // 있는게 없다. 물주기 or 수확의 결과
        AlreadyWatered,       // 이미 물을 줬다
        AlreadyGrown,       // 다 자라서 물 줄 필요가 없음
        NotGrown,           // 아직 덜 자라서 수확 불가능
        NoSeed,             // 씨앗이 없어서 심을게 없음
        UnknownError
    }

    public static class FarmResultText
    {
        public static string Of(FarmResult result)
        {
            return result switch
            {
                FarmResult.Ok => "됐다",
                FarmResult.WrongTile => "여기서는 할 수 없다",
                FarmResult.AlreadyTilled => "이미 갈아 둔 밭이다",
                FarmResult.AlreadyPlanted => "이미 심겨 있다",
                FarmResult.NoCrop => "심긴 것이 없다",
                FarmResult.AlreadyWatered => "오늘은 이미 물을 줬다",
                FarmResult.AlreadyGrown => "다 자랐다. 거두면 된다",
                FarmResult.NotGrown => "아직 덜 자랐다",
                FarmResult.NoSeed => "씨앗이 없다. 상점에서 사야 한다",
                _ => "알 수 없는 에러",
            };
        }
    }
    
    
    
    // 밭에서 벌어지는 모든 일을 맡는다. 씬에 하나만 놓고 씁니다
    // 역할
    // - 갈고
    // - 심고
    // - 물주고
    // - 수확하고
    // 농장을 돌리는 게임 로직의 모음집 역할을 하는 클래스 입니다
    // 모든 작물, 그리드, 타일에 배치될 프롭 들을 총 관리 한다.
    // (원래는 쪼개져있었다)
    
    // 기능구현을 다 하고 Farmer랑 연결만 시켜주면 된다
    
    public class Farm : MonoBehaviour
    {
        [Header("Crops")] 
        [SerializeField] private Crop[] cropPrefabs;

        private WorldClock clock;

        private MapData MapData { get; set; }
        private FarmGrid Grid { get; set; }

        // Farm 구동을 위해서는 아래의 두가지 객체가 꼭 필요함\
        // 초기화시 넣어줄것
        public void Init(MapData mapData, FarmGrid grid)
        {
            MapData = mapData;
            Grid = grid;
        }
        
        # region Unity Method

        private void OnEnable()
        {
            clock = WorldClock.Instance;
            clock.OnDayEnded += HandleDayEnded;
        }

        private void OnDisable()
        {
            if(clock != null) clock.OnDayEnded -= HandleDayEnded;
        }

        #endregion
        
        # region 네 가지 행동

        // 함수 오버로딩을 이용한 편의성 함수
        public FarmResult Till(Vector3 worldPosition)
        {
            Vector2Int coord = Grid.WorldToCell(worldPosition);
            return Till(coord);
        }
        
        // 밭을 가는 해동. (흙 -> 밭)
        public FarmResult Till(Vector2Int coord)
        {
            Tile tile  = Grid.GetTileOrNull(coord);
            
            if(tile == null) return FarmResult.WrongTile;                     // 타일이 없다면
            if(tile.Type == TileType.Tilled) return FarmResult.AlreadyTilled; // 이미 갈려있는 타입이라면
            if (tile.IsTillable == false) return FarmResult.WrongTile;        // 갈수있타일이 아니라면

            // Tilled 타입의 정의 객체(def)를 가지고 와서
            TileDef tilledTileDef = DataManager.Instance.Tiles.Get(TileType.Tilled);
            // coord좌표의 타일의 타입을 바꿔준다
            tile.ChangeType(tilledTileDef);
            RefreshOverLay(coord);
            return FarmResult.Ok;
        }

        // 좌표와 작물의 정보를 저장하는 딕셔너리
        private readonly Dictionary<Vector2Int, Crop> crops = new();
        // 작물이 심어지면 해당칸의 TitleType을 덮어씌워 주는 용도
        private const TileType BLOCKED_TYPE = TileType.Rock;

        public FarmResult Sow(Vector3 worldPosition, int cropId)
        {
            Vector2Int coord = Grid.WorldToCell(worldPosition);
            return Sow(coord, cropId);
        }
        
        // 씨앗을 심는 행동
        public FarmResult Sow(Vector2Int coord, int cropId)
        {
            Tile tile  = Grid.GetTileOrNull(coord);
            if (tile == null) return FarmResult.WrongTile;  // 타일이 없다면
            if (tile.Type != TileType.Tilled) return FarmResult.WrongTile; // 갈려있지 않다면
            if (crops.ContainsKey(coord)) return FarmResult.AlreadyPlanted; // coord 좌표에 작물이 있다면

            Crop sample = null;
            foreach (Crop prefab in cropPrefabs)
            {
                if (prefab.ID == cropId) sample = prefab;
            }
            
            if(null == sample) return FarmResult.UnknownError;
            
            // 타일 가져오고
            Transform tileTransform = MapData.GetTileTransformOrNull(coord);
            // cropPrefab을 이용해서 똑같은 작물 생성해주고, 타일의 자식으로 배치
            Crop crop = Instantiate(sample, tileTransform);
            // 생성한 작물의 로컬좌표를 0,0,0 으로 초기화
            crop.transform.localPosition = Vector3.zero;
            // 작물을 관리하는 딕셔너리에 넣고
            crops.Add(coord, crop);
            // 종료
            return FarmResult.Ok;
        }

        public FarmResult Water(Vector3 worldPosition)
        {
            Vector2Int coord = Grid.WorldToCell(worldPosition);
            return Water(coord);
        }
        
        // 물을 주는 행동
        public FarmResult Water(Vector2Int coord)
        {
            // coord 좌표에 crop이 있는지 부터 검색을 한다
            if(crops.ContainsKey(coord) == false) return FarmResult.NoCrop;
            Crop crop = crops[coord];
            
            // 식물이 다 자랐다면 물을 주지 못한다
            if(crop.IsGrown) return FarmResult.AlreadyGrown;
            // 오늘 물을 줬으면 물을 더이상 못준다
            if (crop.WateredToDay) return FarmResult.AlreadyWatered;
            
            // 다 통과 했으면? 물을 준다
            crop.Water();
            return FarmResult.Ok;
        }

        public FarmResult Harvest(Vector3 worldPosition, out Crop harvested)
        {
            Vector2Int coord = Grid.WorldToCell(worldPosition);
            return Harvest(coord, out harvested);
        }
        
        // 다 자란 작물을 거둔다. out 키워드를 이용해서 수확한 작물 반환해 준다.
        // (원래는 : 수확한 작물의 Def만 반환해 준다)
        public FarmResult Harvest(Vector2Int coord, out Crop harvested)
        {
            harvested = null;
            // coord 좌표가 유효한 좌표인지 검증을 한다.
            if(crops.ContainsKey(coord) == false) return FarmResult.NoCrop;
            Crop crop = crops[coord];

            // 다 자라지 않았다면 실패처리
            if (crop.IsGrown == false) return FarmResult.NotGrown;
            
            // 이렇게 반환하면 맵 상에 있는 crop 게임오브젝트를 반환하는 꼴이 된다.
            // harvested = crop;

            // cropPrefabs에서 원본 객체를 찾아서 반환을 해준다
            // CropDTO -> CropDef -> Crop 구조였다면 CropDef만 반환을 하면 됐지만
            // 현재는 Crop이 데이터를 들고 있기 때문에 엉성한 구조가 됐다.
            for (int i = 0; i < cropPrefabs.Length; i++)
            {
                if (crop.ID == cropPrefabs[i].ID) 
                    harvested = cropPrefabs[i];
            }
            
            crops.Remove(coord);        // 딕셔너리에서 제거하고
            Destroy(crop.gameObject);   // 씬에서 삭제해준다
            
            return FarmResult.Ok;
        }
        
        # endregion

        #region 오버레이

        private class Overlay
        {
            public GameObject Object;
            public TileType Type;
        }

        private readonly Dictionary<Vector2Int, Overlay> overlays = new();
        private readonly Dictionary<string, GameObject> overlaysPrefabs = new(); // 오버레이 견본들. 땜빵
        
        // coord 칸의 덧그림을 종류에 맞춘다.
        // 무엇을 씌울지는 코드가 정하지 않음. tiles.tsv에 별도의 OverlayKey를 활용함
        
        
        private void RefreshOverLay(Vector2Int coord)
        {
            // Null을 반환할수도 있으므로 coord 좌표의 바운드 체크를 먼저 해준다.
            if (Grid.InBounds(coord) == false) return;
            TileDef def = Grid.GetTileOrNull(coord).Def;
            
            // 처리할 오버레이 객체가 없다면
            if (string.IsNullOrEmpty(def.OverlayKey)) return;
            
            // Overlay가 존재하고, 타입도 동일(Def의 타입)하면 return;
            if (overlays.TryGetValue(coord, out Overlay current))
            {
                if (current.Type == def.TileType) return;
            }
            
            // --- 밑에서부터는 바꾸거나 생성하는 로직 ---
            RemoveOverlay(coord); // 오버레이 비워주고

            // 땜빵용으로 넣어준거임 아래 두줄
            if (overlaysPrefabs.ContainsKey(def.OverlayKey) == false)
                overlaysPrefabs.Add(def.OverlayKey, Resources.Load<GameObject>(def.OverlayKey));
            
            // 오버레이 프리펩 골라주고
            GameObject prefab =  overlaysPrefabs[def.OverlayKey];
            // 배치할 타일 트랜스폼 가져오고
            Transform tileTransform = MapData.GetTileTransformOrNull(coord);
            // tileTransform의 자식으로 생성해준다
            GameObject overlay = Instantiate(prefab, tileTransform);
            // overlay의 로컬 좌표를 0,0,0으로 변경해서 부모(tileTransform)와 동일한 위치에 있도록 수정한다
            overlay.transform.localPosition = Vector3.zero;
            // overlays 딕셔너리 coord키에 새로운 Overlay 객체를 생성해서 넣어준다
            overlays[coord] = new Overlay(){Object = overlay, Type = def.TileType};
        }

        // coord 좌표에 오버레이가 없다면 return
        // 있다면 삭제처리한다
        private void RemoveOverlay(Vector2Int coord)
        {
            if (overlays.TryGetValue(coord, out Overlay current) == false) return;
            Destroy(current.Object);
            overlays.Remove(coord);
        }

        #endregion


        private void HandleDayEnded(int day)
        {
            foreach (Crop crop in crops.Values)
            {
                crop.Grow();
            }
        }
        
    }
}