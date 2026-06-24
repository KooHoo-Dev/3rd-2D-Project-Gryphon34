using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;


// 클래스명 중복방지를 위해 namespace를 사용합니다
namespace Study.Input 
{
    public class Study_Input : MonoBehaviour
    {
        private void Update()
        {
            SpaceInputTest();
            ArrowInputTest();
        }

        private void SpaceInputTest()
        {
            // 키보드에 Space Input을 받아봅시다!

            //bool isPressed = Keyboard.current.spaceKey.isPressed;
            //Debug.Log($"isPressed = {isPressed}");

            // wasPressedThisFrame
            // - 해당 키가 눌려있지 않은 상태에서 눌릭 상태가 되었는지 체크합니다.
            // - 키보드의 입력이 시작되었을때 True를 반환합니다.
            if (Keyboard.current.spaceKey.wasPressedThisFrame == true)
            {
                Debug.Log("스페이스키 입력이 시작되었습니다!");
            }
            // isPressed
            // - 해당 키가 눌려있는 상태인지 체크합니다.
            // - 눌려있다면 True, 아니라면 False
            else if (Keyboard.current.spaceKey.isPressed == true)
            {
                Debug.Log("스페이스키가 눌려 있습니다!");  
            }
            // wasReleasedThisFrame
            // - 해당 키가 눌려있는 상태에서 해제 되었는지 체크합니다.
            // - 키보드의 입력이 종료되었을때 True를 반환합니다
            else if (Keyboard.current.spaceKey.wasReleasedThisFrame == true)
            {
                Debug.Log("스페이스키 입력이 종료되었습니다!");
            }

        }

        // 유니티에서는 public 접근제한자를 이용해서 다른 객체에 대한
        // 의존성을 Inspector에서 부여할 수 있습니다.
        // - 의존성주입(DI)이 아님을 유의하십시요
        public GameObject target;

        private void ArrowInputTest()
        {
            // 왼쪽 이동 코드 : Target.transform.position += new Vector3(-1, 0, 0); 
            // 오른쪽 이동 코드 : Target.transform.position += new Vector3(1, 0, 0);
            // 위쪽 이동 코드 : Target.transform.position += new Vector3(0, 1, 0);
            // 아래쪽 이동 코드 : Target.transform.position += new Vector3(0, -1, 0);

            // # 실습
            // 키보드의 화살표를 이용해서 Target 게임오브젝트의
            // 위치를 변경되는 코드를 맹글어 보십쇼

            if (Keyboard.current.leftArrowKey.wasPressedThisFrame == true)
            {
                target.transform.position += new Vector3(-1, 0, 0);
            }
            else if (Keyboard.current.rightArrowKey.wasPressedThisFrame == true)
            {
                target.transform.position += new Vector3(1, 0, 0);
            }
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame == true)
            {
                target.transform.position += new Vector3(0, 1, 0);
            }
            else if (Keyboard.current.downArrowKey.wasPressedThisFrame == true)
            {
                target.transform.position += new Vector3(0, -1, 0);
            }
        }
    }
}