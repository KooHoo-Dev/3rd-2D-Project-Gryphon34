using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Study.MiniDefence
{
    // 배치된 아군 유닛을 클릭했을 때 뜨는 "관리 메뉴". (정보 확인 / 판매)
    // 이 메뉴는 계산을 하지 않는다.
    public class ManageMenu : MonoBehaviour
    {
        [Header("표시")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text statText;
        [SerializeField] private TMP_Text sellRefundText;

        [Header("조작")]
        [SerializeField] private Button sellButton;

        private Unit target;   // 지금 관리 중인 유닛

        private void Awake()
        {
            sellButton.onClick.AddListener(OnSellClicked);
        }

        public void Open(Unit unit)
        {
            target = unit;

            gameObject.SetActive(true);

            Refresh();
            UIManager.Instance.ShowRange(unit.transform.position, unit.Range);
        }

        public void Close()
        {
            target = null;
            gameObject.SetActive(false);
        }

        private void OnSellClicked()
        {
            PlacementSystem.Instance.Remove(target.Coord);

            // 유닛이 사라졌으므로 메뉴도 닫는다.
            UIManager.Instance.CloseAll();
        }

        private void Refresh()
        {
            titleText.text = target.Def.Name;

            statText.text = $"피해 {target.Damage} · 사거리 {target.Range}\n" +
                            $"공격 간격 {target.AttackInterval}초 · 크리 {target.CritChance:P0}";

            sellRefundText.text = $"판매  +{PlacementSystem.Instance.GetRefund(target)}";
        }
    }
}
