using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Study.MiniDefence
{
    public class HUD : MonoBehaviour
    {
        [Header("표시")] 
        [SerializeField] private TMP_Text mineralText;
        
        [SerializeField] private TMP_Text livesText;
        [SerializeField] private TMP_Text waveText;
        [SerializeField] private TMP_Text prepText;

        [Header("조작 버튼")]
        [SerializeField] private Button nextWaveButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;
        
        private void Start()
        {
            RefreshMinerals(EconomyManager.Instance.Minerals);
            RefreshLives(GameManager.Instance.Lives);
            RefreshWave(WaveManager.Instance.CurrentWaveNo);
            
            // RefreshMinerals 함수를 MineralsChanged에 연결 시켜준다
            EconomyManager.Instance.MineralsChanged += RefreshMinerals;
            // RefreshLives 함수를 GameManager에 연결 시켜준다
            GameManager.Instance.LivesChanged += RefreshLives;
            // RefreshWave 함수를 WaveManager에 연결 시켜준다
            WaveManager.Instance.WaveChanged += RefreshWave;
            
            saveButton.onClick.AddListener(SaveService.Save);
            loadButton.onClick.AddListener(SaveService.Load);
        }

        private void Update()
        {
            nextWaveButton.interactable = GameManager.Instance.State.CanSave;
            prepText.SetText($"남은 시간 : {GameManager.Instance.PrepRemaining:F2}");
        }

        private void OnDestroy()
        {
            if(EconomyManager.Instance != null)
                EconomyManager.Instance.MineralsChanged -= RefreshMinerals;
        }

        // amount = 잔액
        private void RefreshMinerals(int amount)
        {
            // ".text = " => 가비지가 나옴 
            mineralText.SetText($"광물 : {amount}");
        }

        private void RefreshLives(int lives)
        {
            livesText.SetText($"체력 : {lives}");
        }

        private void RefreshWave(int wave)
        {
            int totalCount = DataManager.Instance.Waves.Count;
            waveText.SetText($"웨이브 : {wave}/{totalCount}");   
        }
        
        #region 버튼 연동 함수들

        private bool CanCallNextWave()
        {
            return true;
        }

        public void OnClickNextWave()
        {
            // 테스트 용도로 맹글었음. 바꿔줘야함
            int nextWaveID = WaveManager.Instance.CurrentWaveNo + 1;
            var wave = DataManager.Instance.GetWaveDef(nextWaveID);
            var map = BoardMap.Instance.Map;
            WaveManager.Instance.StartWave(wave, map);
        }

        #endregion
    }
}