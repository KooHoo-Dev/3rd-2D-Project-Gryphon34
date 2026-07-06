using UnityEngine;
using UnityEngine.SceneManagement;

namespace Geometry
{
    public class MainMenu : MonoBehaviour
    {
        public void OnInfoClickButton()
        {
            SceneManager.LoadScene("01. Info");
        }

        public void OnGameClickButton()
        {
            SceneManager.LoadScene("02. Game");
        }
    }
}

