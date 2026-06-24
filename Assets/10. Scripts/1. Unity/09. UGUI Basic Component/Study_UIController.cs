using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Study.UGUI_BasicComponent
{
    public class Study_UIController : MonoBehaviour
    {
        // 스크립트를 이용해서 여러 캔버스를 제어하는 방법을 알아봅시다.
        // 1. 키입력으로 제어하기
        // 2. Button을 사용해서 UI 입력을 통해 제어하기

        public enum CanvasType
        {
            CanvasA=1,
            CanvasB,
            CanvasC,
            CanvasD,
        }

        private Canvas[] canvases;
        private Canvas menuCanvas;

        private void Awake()
        {
            canvases = GetComponentsInChildren<Canvas>();
            
            // 여러 캔버스 중 MenuCanvas 게임 오브젝트를 골라서
            // menuCanvas에 할당을 해줍니다. 해당 Canvas는 언제나 활성화 해둘겁니다.

            for(int i = 0; i < canvases.Length; ++i)
            {
                if (canvases[i].gameObject.name.Equals("MenuCanvas"))
                {
                    menuCanvas = canvases[i];
                    break;
                }
            }
            SetActiveCanvas(CanvasType.CanvasA);
            SetButtons();
        }

        private void Update()
        {
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                SetActiveCanvas(CanvasType.CanvasA);
            }

            if (Keyboard.current.wKey.wasPressedThisFrame)
            {
                SetActiveCanvas(CanvasType.CanvasB);
            }

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                SetActiveCanvas(CanvasType.CanvasC);
            }

            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                SetActiveCanvas(CanvasType.CanvasD);
            }
        }

        public void SetActiveCanvas(CanvasType canvasType)
        {
            for(int i = 0; i < canvases.Length; ++i)
            {
                canvases[i].enabled = false;
            }

            // 켜줘야 할 녀석들만 켜준다
            menuCanvas.enabled = true;
            canvases[(int)canvasType].enabled = true;
        }

        public void SetActiveCanvas(int canvasType)
        {
            SetActiveCanvas((CanvasType)canvasType);
        }

        private Button[] buttons;

        private void SetButtons()
        {
            buttons = menuCanvas.transform.GetComponentsInChildren<Button>();

            for(int i = 0; i < buttons.Length; ++i)
            {
                // 아래 코드는 인스펙터에서 OnClick 박스의 +를 나타냄
                int index = i+1;
                buttons[i].onClick.AddListener(()=>SetActiveCanvas(index));
                // 람다 표현식 : (매개변수) => 지칭 함수
                // : 프로그래밍에서 함수를 하나의 식으로 간결하게 표현하는 방법
                //   익명함수, 무명함수(이름이 없는 함수)라고도 부르고, 코드의
                //   가독성을 높이지만, 비용(가비지)이 소모됩니다.

                // 캡처
                // : 람다가 선언된 범위 밖의 외부 변수를 람다 내부로 가져와서 사용하는
                //   동작을 의미합니다. 값 캡처와 참조 캡처가 있어서 캡처가 일어날 경우
                //   의도치 않은 버그가 발생할 수 있습니다.
                //
            }
        }
    }
}