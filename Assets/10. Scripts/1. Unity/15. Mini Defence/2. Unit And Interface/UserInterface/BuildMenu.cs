using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Study.MiniDefence
{
    // 빈 건설 패드를 클릭했을 때 뜨는 "유닛 생성 메뉴".
    // 칸을 인스펙터에 손으로 3개 만들어 두지 않고 데이터에서 만든다.
    public class BuildMenu : MonoBehaviour
    {
        [SerializeField] private BuildMenuSlot slotPrefab;
        [SerializeField] private Transform slotParent;
        [SerializeField] private Button buildButton;
        [SerializeField] private TMP_Text infoText;

        private readonly List<BuildMenuSlot> slots = new();

        private Vector2Int targetCoord;    // 어느 타일에 지을 것인가
        private UnitDef selectedDef;       // 무엇을 지을 것인가

        private void Awake()
        {
            buildButton.onClick.AddListener(OnBuildClicked);
        }

        // 열려 있는 동안에는 광물 변화를 따라가며 "살 수 있는지"를 갱신한다.
        private void OnEnable()
        {
            if (EconomyManager.IsInitialized)
                EconomyManager.Instance.MineralsChanged += HandleMineralsChanged;
        }

        private void OnDisable()
        {
            if (EconomyManager.IsInitialized)
                EconomyManager.Instance.MineralsChanged -= HandleMineralsChanged;
        }

        public void Open(Vector2Int coord)
        {
            targetCoord = coord;

            gameObject.SetActive(true);

            EnsureSlots();
            Select(null);   // 아직 아무것도 고르지 않은 상태로 시작
        }

        public void Close()
        {
            // 확정하지 않고 닫혔으므로 세워 보던 미리보기도 치운다. (= 취소)
            PlacementSystem.Instance.ClearPreview();

            gameObject.SetActive(false);
        }

        // 데이터에 있는 유닛 종류만큼 칸을 만든다.
        private void EnsureSlots()
        {
            if (slots.Count > 0) return;

            foreach (UnitDef def in DataManager.Instance.Units.All)
            {
                BuildMenuSlot slot = Instantiate(slotPrefab, slotParent);
                slot.Bind(def, Select);

                slots.Add(slot);
            }
        }

        // 칸을 클릭했을 때. (null이면 선택 해제)
        private void Select(UnitDef def)
        {
            selectedDef = def;

            foreach (BuildMenuSlot slot in slots)
                slot.SetSelected(slot.Def == def);

            // 고른 유닛을 그 자리에 반투명하게 세워 본다. 아직 확정은 아니다.
            if (def == null) PlacementSystem.Instance.ClearPreview();
            else PlacementSystem.Instance.ShowPreview(def, targetCoord);

            RefreshAffordability();
            RefreshInfo();
        }

        private void OnBuildClicked()
        {
            if (selectedDef == null) return;

            Unit unit = PlacementSystem.Instance.Build(selectedDef, targetCoord);
            if (unit == null) return;   // 실패(광물 부족 등)하면 메뉴를 그대로 열어 둔다

            UIManager.Instance.CloseAll();
        }

        private void HandleMineralsChanged(int minerals)
        {
            RefreshAffordability();
        }

        // 지금 광물로 살 수 있는 칸만 켜 둔다.
        private void RefreshAffordability()
        {
            foreach (BuildMenuSlot slot in slots)
                slot.SetAffordable(EconomyManager.Instance.CanAfford(slot.Def.Cost));

            buildButton.interactable = (selectedDef != null)
                && EconomyManager.Instance.CanAfford(selectedDef.Cost);
        }

        // 선택한 유닛의 정보와, 지어질 자리의 사거리 미리보기를 보여준다.
        private void RefreshInfo()
        {
            if (selectedDef == null)
            {
                infoText.text = "유닛을 선택하세요";
                UIManager.Instance.HideRange();
                return;
            }

            infoText.text = $"{selectedDef.Name}   비용 {selectedDef.Cost}\n" +
                            $"피해 {selectedDef.Damage} · 사거리 {selectedDef.Range} · 간격 {selectedDef.AttackInterval}초";

            UIManager.Instance.ShowRange(BoardMap.Instance.CellToWorld(targetCoord), selectedDef.Range);
        }
    }
}