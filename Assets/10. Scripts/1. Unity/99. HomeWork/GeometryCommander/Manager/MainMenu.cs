using UnityEngine;
using UnityEngine.SceneManagement;

namespace Geometry
{
    public class MainMenu : MonoBehaviour
    {
        public GameObject infoWindow;

        private void Awake()
        {
            infoWindow.SetActive(false);
        }
        public void OnInfoClickButton()
        {
            infoWindow.SetActive(true);
        }

        public void OnGameClickButton()
        {
            infoWindow.SetActive(false);
            SceneManager.LoadScene("01. Game");
        }
    }
}

