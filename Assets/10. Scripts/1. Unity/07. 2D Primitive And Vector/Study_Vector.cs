using UnityEngine;
using UnityEngine.InputSystem;

namespace Study.PrimitiveAndVector
{
    // # Vector?
    // 3차원 공간에서 위치, 방향(or 크기)를 나타내는 데 사용되는 구조체(Struct)
    // 마치, 지도에서 특정 지점의 좌표를 표시하거나, 어떤 방향으로 어떤 속도 만큼
    // 이동하는지를 표현하는데에 사용됩니다.

    public class Study_Vector : MonoBehaviour
    {
        // # 3D 그래픽스(게임) 엔진에서 자주 사용하는
        // Vector3 관련 정적 필드와 메서드

        // Vector3.zero : (0,0,0)을 담아둔 정적 변수()(원점, 크기가 없는 상태)
        // Vector3.one : (1,1,1)을 담아둔 변수(유니티 게임 오브젝트의 기본 Scale값)

        // Direction
        // Vector3.forward : (0,0,1)      (Z축의 양의 방향)
        // Vector3.back :    (0,0,-1)     (Z축의 음의 방향)
        // Vector3.up :      (0,1,0)      (Y축의 양의 방향)
        // Vector3.down :    (0,-1,0)     (Y축의 음의 방향)
        // Vector3.right :   (1,0,0)      (X축의 양의 방향)
        // Vector3.left :    (-1,0,0)     (X축의 음의 방향)

        // # 길이
        // vector.magnitude :       벡터의 길이(크기)를 반환합니다 (원점으로 부터 얼마나 떨어졌는지)
        // vector.sqrMagnitude :    벡터의 길이의 제곱을 반환합니다
        //                          magnitude보다 연산 비용이 적어서 단순 비교시 사용함
        // vector.normalized :      길이가 1인 단위 벡터(Unit Vector)를 반환
        //                          방향을 나타낼 때
        
        private void Update()
        {
            Vector();
        }


        public void Vector()
        {
            // Vector의 합연산
            // - 의미 : 길이, 방향을 변환시키고자 할때

            // Vector의 곱연산
            // - 의미 : 길이를 변환시키고자 할때

            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                // transform은 모든 GameObject에 존재함
                transform.position += Vector3.one;
            }
            if (Keyboard.current.wKey.wasPressedThisFrame)
            {
                Vector3 angle = new Vector3(0, 0, 30);
                transform.rotation *= Quaternion.Euler(angle);
            }
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                Vector3 applyVector = Vector3.one + Vector3.one;
                transform.localScale += applyVector;
            }
        }
    }

}