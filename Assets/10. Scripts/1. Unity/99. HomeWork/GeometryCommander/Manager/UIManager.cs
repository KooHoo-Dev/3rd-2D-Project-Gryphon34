using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Geometry
{
    public class UIManager : MonoBehaviour
    {
        public PlayerManager playerManager;
        public GameManager gameManager;
        public TextMeshProUGUI supplyText;
        public TextMeshProUGUI costText;
        public TextMeshProUGUI goalTimeText;
        public TextMeshProUGUI TimeText;
        public Slider triangleCooldown;
        public Slider circleCooldown;
        public Slider squareCooldown;
        public Slider supportCooldown;


        void Update()
        {
            if(playerManager != null&&gameManager !=null)
            {
                if (gameManager.IsgameOver)
                {
                    TimeText.text = goalTimeText.text;
                }
                supplyText.text = $"Supply : {playerManager.currentSupply} / {playerManager.maxSupply}";

                costText.text = $"Cost : {playerManager.currentCost} / {playerManager.maxCost}";

                goalTimeText.text = $"Time Left : {(int)gameManager.goalTime}";

                // 1. Triangle 쿨타임 업데이트
                if (playerManager.UnitCooldownDic.TryGetValue(PlayerManager.UnitType.Triangle, out float triProgress))
                    triangleCooldown.value = triProgress;
                else
                    triangleCooldown.value = 1f;

                // 2. Circle 쿨타임 업데이트
                if (playerManager.UnitCooldownDic.TryGetValue(PlayerManager.UnitType.Circle, out float cirProgress))
                    circleCooldown.value = cirProgress;
                else
                    circleCooldown.value = 1f;

                // 3. Square 쿨타임 업데이트
                if (playerManager.UnitCooldownDic.TryGetValue(PlayerManager.UnitType.Square, out float sqProgress))
                    squareCooldown.value = sqProgress;
                else
                    squareCooldown.value = 1f;

                // 4. Support 쿨타임 업데이트
                if (playerManager.UnitCooldownDic.TryGetValue(PlayerManager.UnitType.Support, out float supProgress))
                    supportCooldown.value = supProgress;
                else
                    supportCooldown.value = 1f;
            }
        }

        public void OnButtonClick()
        {
            SceneManager.LoadScene("00. MainMenu");
        }
        
    }
}

