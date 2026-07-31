using Study.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Study.MiniDefence
{
    // 마우스 클릭을 "게임의 의미"로 바꿔서 UI에 넘긴다.
    // 역할을 이렇게 나눠 둔다.
    //   InputHandler(어디를 눌렀나) → UIManager(무엇을 열까) → 각 메뉴(무엇을 할까)
    public class InputHandler : MonoBehaviour
    {
        [SerializeField] private Camera worldCamera;

        private void Awake()
        {
            if (worldCamera == null) worldCamera = Camera.main;
        }

        private void Update()
        {
            if (SimpleInput.GetMouseButtonDown(SimpleInput.MouseButton.Left) == false) return;

            // 승리·패배 뒤에는 보드를 만질 수 없다.
            //if (GameManager.Instance.State.CanBuild == false) return;

            // 버튼 같은 UI 위에서 누른 것이라면 월드를 클릭한 게 아니다.
            // (이 처리를 빼먹으면 메뉴의 버튼을 누를 때마다 뒤쪽 타일까지 함께 클릭된다)
            if (IsPointerOverUI()) return;

            Tile tile = BoardMap.Instance.GetTileAtWorldOrNull(GetMouseWorldPosition());

            if (tile == null)
            {
                UIManager.Instance.CloseAll();   // 보드 바깥을 클릭하면 열린 메뉴를 닫는다
                return;
            }

            UIManager.Instance.HandleTileClicked(tile);
        }

        private bool IsPointerOverUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }

        private Vector3 GetMouseWorldPosition()
        {
            // 화면 좌표 -> 월드 좌표.
            // 2D(직교) 카메라라도 z를 카메라와의 거리로 넣어주는 것이 안전하다.
            Vector3 screenPosition = SimpleInput.GetMousePosition();
            screenPosition.z = -worldCamera.transform.position.z;

            return worldCamera.ScreenToWorldPoint(screenPosition);
        }
    }
}