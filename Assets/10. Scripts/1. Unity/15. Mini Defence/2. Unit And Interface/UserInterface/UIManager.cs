using Jay;
using UnityEngine;

namespace Study.MiniDefence
{
    // UI들의 "교통정리" 담당.
    //
    // 클릭 한 번에 무엇을 열지 정하는 규칙이 여기 한곳에 모여 있다.
    //   빈 건설 패드  → 생성 메뉴
    //   아군 유닛     → 관리 메뉴 + 사거리 표시
    //   그 밖의 타일  → 배치 불가 피드백
    //
    // 각 메뉴는 서로를 알지 못한다. 열고 닫는 것은 모두 이 객체를 거친다.
    // (메뉴끼리 직접 부르기 시작하면 금방 얽혀서 손대기 어려워진다)
    public class UIManager : SingletonBase<UIManager>
    {
        [SerializeField] private BuildMenu buildMenu;
        [SerializeField] private ManageMenu manageMenu;
        [SerializeField] private RangeIndicator rangeIndicator;

        // 타일 하나를 클릭했다. 그 타일의 상태를 보고 무엇을 열지 정한다.
        public void HandleTileClicked(Tile tile)
        {
            CloseAll();

            // 1. 유닛이 있는 타일 -> 관리 메뉴
            if (tile.Unit != null)
            {
                manageMenu.Open(tile.Unit);
                return;
            }

            // 2. 비어 있는 건설 패드 -> 생성 메뉴
            if (tile.IsBuildable())
            {
                buildMenu.Open(tile.Coord);
                return;
            }

            // 3. 배치할 수 없다는 피드백
            Debug.Log($"[UIManager] {tile.Coord} 에는 배치할 수 없습니다. (타일 종류 : {tile.Kind})");
        }

        public void CloseAll()
        {
            buildMenu.Close();
            manageMenu.Close();
            HideRange();
        }

        #region 사거리 표시

        public void ShowRange(Vector3 center, float radius)
            => rangeIndicator.Show(center, radius);

        public void HideRange()
            => rangeIndicator.Hide();

        #endregion
    }
}
