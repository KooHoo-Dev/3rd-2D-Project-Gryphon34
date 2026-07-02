using Study.Utilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Geometry
{
    public class MainMenu : MonoBehaviour
    {
        public void OnClickButton()
        {

            SceneManager.LoadScene("00. Game");
}
    }
}

