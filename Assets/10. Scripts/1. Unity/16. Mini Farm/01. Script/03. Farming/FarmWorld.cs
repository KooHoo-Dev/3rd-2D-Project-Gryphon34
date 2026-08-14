using Jay;
using UnityEngine;

namespace Study.MiniFarm
{
    // 이 씬(맵 + 농사 그리드 + a)이 어떻게 생겼는가
    // 전체를 관장 할 수 있는 관리자 객체
    // 하는일은 아래와 같습니다
    // - 맵을 선택한다
    // - 농사 그리드를 만든다
    // - 농사 시스템 준비되었음을 알린다
    // - 데이터등을 나눠준다 : 그리드, 맵, 좌표 변환 기타 등등
    
    // 하지않는 일은 아래와 같습니다
    // - 길을 찾지 않는다          : 길찾기는 PathFinder
    // - 표(시트)를 일지 않습니다   : DataManager가
    // - 농사 규칙을 모릅니다       : 농사 규칙은 Farm이
    
    // 싱글톤으로 해도 상관은 없음
    public class FarmWorld : MonoBehaviour
    {
        public MapData MapData { get; private set; }
        public MapLoader MapLoader { get; private set; }
        public FarmGrid Grid { get; private set; }
        public bool IsReady => Grid != null; // Grid가 있으면 준비 완료 판정


        protected void Awake()
        {
            BuildWorld();
        }

        private async void BuildWorld()
        {
            await DataManager.Instance.EnsureLoadedAsync();
            MapLoader = GetComponent<MapLoader>();
            // 맵을 불러와서 생성한다
            MapData = MapLoader.LoadOrNull();
            if (MapData == null)
            {
                Debug.LogError("[FarmWorld] MapData가 생성되지 않았습니다");
                return;
            }
            Grid = MapData.ToGrid(DataManager.Instance.Tiles);

            // Farm  초기화 절차
            Farm farm = FindAnyObjectByType<Farm>();
            if (farm == null)
            {
                Debug.LogError("[FarmWorld] 씬 내에 Farm 컴포넌트가 없음");
                return;
            }
            
            farm.Init(MapData, Grid);
            
            PathFinder pathFinder = FindAnyObjectByType<PathFinder>();
            if (pathFinder == null)
            {
                Debug.LogError("[FarmWorld] 씬 내에 PathFinder 컴포넌트가 없음");
                return;
            }
            
            pathFinder.Init(Grid);
            
            FarmerCommander commander = FindAnyObjectByType<FarmerCommander>();
            commander.Init(pathFinder);
        }
    }
}