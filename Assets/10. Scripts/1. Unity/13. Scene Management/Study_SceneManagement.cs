using Jay;
using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Study_SceneManagement
{
    // # 동기적 씬 로딩
    // - 가장 간단한 방법으로 유니티에서 가장 많이 사용합니다.
    // - 씬의 모든 요소들을 불러오며, 불러오자마자 현재 씬을 바꿔줍니다.
    public class Study_SceneManagement : SingletonBase<Study_SceneManagement>
    {
        private Canvas canvas;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            canvas = GetComponent<Canvas>();
        }
        private void Update()
        {
            // 탭을 눌렀을 때만 Canvas가 출력되게 해줍니다.
            bool isPressed = SimpleInput.GetKey(Key.Tab);
            canvas.enabled = isPressed;
            if (isPressed == false) return;

            if (SimpleInput.GetKeyDown(Key.Digit1))
                SceneManager.LoadScene(0);
            if (SimpleInput.GetKeyDown(Key.Digit2))
                SceneManager.LoadScene(1);
            if (SimpleInput.GetKeyDown(Key.Digit3))
                SceneManager.LoadScene("UFO Game");
            if (SimpleInput.GetKeyDown(Key.Digit4))
                SceneManager.LoadScene("Merge Game");
            if (SimpleInput.GetKeyDown(Key.F5))
                SceneManager.LoadScene(4);
        }

        // 보통 씬을 사용하는 구조는 아래와 같습니다.

        public enum GameScene
        {
            CardSelector = 0,
            PairMatching,
            UFOGame,
            MergeGame,
            Home
        }

        private GameScene PrevScene { get; set; }
        private GameScene CurrentScene { get; set; } = GameScene.CardSelector;

        public void ChangeScene(int sceneIndex)
        {
            ChangeScene((GameScene)sceneIndex);
        }

        public void ChangeScene(GameScene targetScene)
        {
            PrevScene = CurrentScene;
            CurrentScene = targetScene;
            SceneManager.LoadScene((int)CurrentScene);
        }

        public void MovePrevScene()
        {
            ChangeScene(PrevScene);
        }
    }
}