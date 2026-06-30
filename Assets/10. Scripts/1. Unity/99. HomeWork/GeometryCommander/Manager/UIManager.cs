using UnityEngine;
using TMPro;

namespace Geometry
{
    public class UIManager : MonoBehaviour
    {
        public TextMeshProUGUI supplyText;
        public TextMeshProUGUI costText;

        
        void Update()
        {
            if(PlayerManager.Instance != null)
            {
                supplyText.text = $"Supply : {PlayerManager.Instance.currentSupply} / {PlayerManager.Instance.maxSupply}";

                costText.text = $"Cost : {PlayerManager.Instance.currentCost} / {PlayerManager.Instance.maxCost}";
            }
        }
    }
}

