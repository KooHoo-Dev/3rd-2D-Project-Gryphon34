using UnityEngine;

namespace Study.MiniFarm
{
    // 게임을 시작할 때 맵 파일을 읽어 씬에 세운다.
    //
    // 프리팹으로 맵을 미리 만들어 두는 방법도 있고 그것도 잘 된다.
    // 그런데 파일로 두면 좋은 점이 있다.
    //   - 맵을 고치는 데 유니티를 열지 않아도 된다
    //   - 맵을 여러 개 만들어 이름만 바꿔 불러올 수 있다
    //   - 나중에 저장,이어하기에서 "그때의 밭 상태"를 같은 형식으로 담을 수 있다
    public class MapLoader : MonoBehaviour
    {
        [Header("불러올 맵")]
        // StreamingAssets/Maps/MiniFarm/{이 이름}.json
        [SerializeField] private string mapName = "Farm01";

        [Header("칸 그림")]
        // 비워 두면 색만 있고 아무것도 안 보인다. 흰 사각형 스프라이트를 넣어 둔다.
        [SerializeField] private Sprite tileSprite;

        public MapData Map { get; private set; }

        public MapData LoadOrNull()
        {
            if (Map != null) return Map;

            MapFile file = MapFile.LoadOrNull(mapName);

            if (file == null) return null;

            Map = MapSpawner.Spawn(file, tileSprite, transform.position, transform);

            Debug.Log($"[MapLoader] '{mapName}' 을(를) 세웠습니다 — {file.Width} x {file.Height} · 물체 {file.Objects.Count}개");

            return Map;
        }
    }
}
