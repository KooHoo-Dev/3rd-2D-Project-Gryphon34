using UnityEngine;
using UnityEngine.InputSystem;

namespace Study.MiniFarm
{
    public class WorldClockTester : MonoBehaviour
    {
        private string currentWeather = "맑음";
        private readonly string[] weatherTypes = { "맑음", "흐림", "비", "눈" };

        private void OnEnable()
        {
            // WorldClock의 이벤트 구독
            if (WorldClock.Instance != null)
            {
                WorldClock.Instance.OnDayStarted += HandleDayStarted;
                WorldClock.Instance.OnDayEnded += HandleDayEnded;
                WorldClock.Instance.OnTick += HandleTick;
                WorldClock.Instance.OnPauseChanged += HandlePauseChanged;
            }
        }

        private void OnDisable()
        {
            // WorldClock의 이벤트 구독 해제
            if (WorldClock.Instance != null)
            {
                WorldClock.Instance.OnDayStarted -= HandleDayStarted;
                WorldClock.Instance.OnDayEnded -= HandleDayEnded;
                WorldClock.Instance.OnTick -= HandleTick;
                WorldClock.Instance.OnPauseChanged -= HandlePauseChanged;
            }
        }

        private void Update()
        {
            // New Input System을 이용한 키 입력 처리
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard.digit1Key.wasPressedThisFrame)
            {
                WorldClock.Instance.SetScaleStep(1);
            }
            else if (keyboard.digit2Key.wasPressedThisFrame)
            {
                WorldClock.Instance.SetScaleStep(2);
            }
            else if (keyboard.digit3Key.wasPressedThisFrame)
            {
                WorldClock.Instance.SetScaleStep(3);
            }
            else if (keyboard.digit4Key.wasPressedThisFrame)
            {
                WorldClock.Instance.TogglePause();
            }
        }

        #region Event Handlers

        private void HandleDayStarted(int day)
        {
            // 하루가 시작될 때 날씨를 무작위로 변경하여 테스트
            currentWeather = weatherTypes[Random.Range(0, weatherTypes.Length)];
            Debug.Log($"[Tester] Day {day} Started! Weather is now {currentWeather}.");
        }

        private void HandleDayEnded(int day)
        {
            Debug.Log($"[Tester] Day {day} Ended.");
        }

        private void HandleTick()
        {
            Debug.Log("[Tester] Tick!");
        }

        private void HandlePauseChanged(bool isPaused)
        {
            Debug.Log($"[Tester] Pause state changed: {(isPaused ? "Paused" : "Resumed")}");
        }

        #endregion

        private void OnGUI()
        {
            if (WorldClock.Instance == null) return;
            GUI.color = Color.black;
            // 화면 좌측 상단에 텍스트 출력
            GUILayout.BeginArea(new Rect(20, 20, 300, 400));
            
            GUILayout.Label("<b>[ WorldClock Status ]</b>");
            GUILayout.Label($"Day: {WorldClock.Instance.Day}");
            GUILayout.Label($"Time Of Day: {WorldClock.Instance.TimeOfDay:F3} (0~1)");
            GUILayout.Label($"Weather: {currentWeather}");
            GUILayout.Label($"Scale Step: {WorldClock.Instance.ScaleStep} (x{WorldClock.Instance.Scale})");
            GUILayout.Label($"Is Paused: {WorldClock.Instance.IsPaused}");
            GUILayout.Label($"Is Finished: {WorldClock.Instance.IsFinished}");
            
            GUILayout.Space(20);
            
            GUILayout.Label("<b>[ Input Controls ]</b>");
            GUILayout.Label("Key 1 : Set Scale Step 1");
            GUILayout.Label("Key 2 : Set Scale Step 2");
            GUILayout.Label("Key 3 : Set Scale Step 3");
            GUILayout.Label("Key 4 : Toggle Pause");

            GUILayout.EndArea();
        }
    }
}