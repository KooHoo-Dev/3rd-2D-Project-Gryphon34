using System;
using Study.Utilities;
using UnityEngine;

namespace Study.MiniFarm
{
    // 농부(캐릭터)의 위치에 따라서 화면 움직이게(스크롤링) 하는 객체 
    // - 화면 가장자리에 마우스가 닿으면 화면을 민다
    // - 화면이 맵 밖으로 나가지 않게 붙잡는다
    //   - 붙잡는게 더 중요한 포인트 입니다 
    [RequireComponent(typeof(Camera))]
    public class CameraScroller : MonoBehaviour
    {
        [Header("Settings")]
        // 맵의 범위를 제한하게 하는 필드
        [SerializeField] private Vector2 mapMin = Vector2.zero;
        [SerializeField] private Vector2 mapMax = new Vector2(24f, 16f); // width, height
        
        // 스크롤 관련 필드
        [SerializeField] private float scrollSpeed = 8.0f;
        [SerializeField, Range(0.01f, 0.3f)] 
        private float edgeThickness = 0.05f; // 화면 경계의 스크롤 판정영역의 두께 

        private Camera cam;
        
        private void Awake()
        {
            cam = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            Vector2 direction = ReadEdgeDirection();
            // Vector 동등비교 1000분의 1인가 100000분의 1인가 암튼 잊지 말것
            if (direction == Vector2.zero) return; 
            
            Vector3 moveVector = direction.normalized * scrollSpeed * Time.deltaTime;
            Vector3 newPosition = transform.position + moveVector;
            transform.position = Clamp(newPosition);
        }

        // 마우스가 있는 경계면을 읽어주는 함수.
        // 마우스 위치와 경계면을 판단하여 알맞은 방향벡터를 반환한다
        private Vector2 ReadEdgeDirection()
        {
            // 마우스 좌표를 가지고 온다
            Vector2 mouse = SimpleInput.GetMousePosition();
            
            // 마우스가 화면 밖에있는지 판단한다. 화면 밖에 있으면
            // Alt + Tap이나 윈도우 모드일때 화면이 계속 스크롤링 된다.
            // 예외 처리 해준다
            //if (mouse.x < 0f || Screen.width < mouse.x) return Vector2.zero;
            //if (mouse.y < 0f || Screen.height < mouse.y) return Vector2.zero;

            // 해상도에 맞춰서(비율로) x, y의 영역을 계산해준다.
            float bandX = Screen.width * edgeThickness;
            float bandY = Screen.height * edgeThickness;

            Vector2 direction = Vector2.zero;

            // 경계면인지 판단한다(안쪽이면 Vector2.Zero로 return해 버린다)
            if (mouse.x < bandX) direction.x = -1f;
            else if (Screen.width - bandX < mouse.x) direction.x = 1f;
            
            if (mouse.y < bandY) direction.y = -1f;
            else if (Screen.height - bandY < mouse.y) direction.y = 1f;

            return direction.normalized;
        }
        
        // --- 아래부터는 경계 붙잡기 로직들 ---

        // 카메라가 보는 사각형이 맵 안에 들어오도록 위치를 조정해서 반환하는 함수.
        private Vector3 Clamp(Vector3 position)
        {
            // orthographic : 직교 카메라. size의 경우 "세로 절반"을 의미합니다.
            float halfHeight = cam.orthographicSize;
            // aspect ratio : 이미지, 화면 혹은 물체의 너비와 높이의 비율을 뜻합니다
            float halfWidth = cam.aspect * halfHeight;

            float x = ClampAxis(position.x, mapMin.x, mapMax.x, halfWidth);
            float y = ClampAxis(position.y, mapMin.y, mapMax.y, halfHeight);

            return new Vector3(x, y, position.z);
        }

        // X,Y 중 한축을 맵 안에 가두는 함수 입니다.
        private float ClampAxis
            (float value, float min, float max, float halfSize)
        {
            // x나 y가 넘어서면 안되는 값을 먼저 설정해줍니다. 
            float limitMin = min + halfSize;
            float limitMax = max - halfSize;
            
            return Mathf.Clamp(value, limitMin, limitMax);
        }
    }
}