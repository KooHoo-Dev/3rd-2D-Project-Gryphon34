using System;
using TMPro;
using UnityEngine;

namespace Study.MiniDefence
{
    public class HUD : MonoBehaviour
    {
        [Header("표시")] 
        [SerializeField] private TMP_Text mineralText;

        private void Start()
        {
            RefreshMinerals(EconomyManager.Instance.Minerals);
            // RefreshMinerals 함수를 MineralsChanged에 연결 시켜준다
            EconomyManager.Instance.MineralsChanged += RefreshMinerals;            
        }

        private void OnDestroy()
        {
            EconomyManager.Instance.MineralsChanged -= RefreshMinerals;
        }

        // amount = 잔액
        private void RefreshMinerals(int amount)
        {
            // ".text = " => 가비지가 나옴 
            mineralText.SetText($"광물 : {amount}");
        }
    }
}