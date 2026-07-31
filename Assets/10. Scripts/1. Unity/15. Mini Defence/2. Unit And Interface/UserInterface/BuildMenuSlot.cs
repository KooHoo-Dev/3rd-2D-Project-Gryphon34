using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Study.MiniDefence
{
    // 생성 메뉴의 칸 하나. 유닛 "한 종류"를 보여준다.
    //
    // 무엇을 보여줄지는 데이터(UnitDef)가 정하고, 이 스크립트는 그리기만 한다.
    // 그래서 유닛이 3종이든 5종이든 이 파일은 바뀌지 않는다.
    public class BuildMenuSlot : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private GameObject selectedMark;   // 선택했을 때 켜지는 강조 표시

        public UnitDef Def { get; private set; }

        // 데이터를 받아 칸을 채운다. 클릭하면 onClicked로 자기 데이터를 넘겨준다.
        public void Bind(UnitDef def, Action<UnitDef> onClicked)
        {
            Def = def;

            nameText.text = def.Name;
            costText.text = def.Cost.ToString();

            icon.sprite = def.Sprite;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClicked(def));

            SetSelected(false);
        }

        // 광물이 모자라면 누를 수 없게 만든다.
        public void SetAffordable(bool canAfford)
        {
            button.interactable = canAfford;
        }

        public void SetSelected(bool selected)
        {
            selectedMark.SetActive(selected);
        }
    }
}