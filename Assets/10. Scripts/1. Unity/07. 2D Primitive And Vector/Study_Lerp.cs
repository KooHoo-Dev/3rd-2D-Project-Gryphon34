using UnityEngine;

namespace Study.PrimitiveAndVector
{
    public class StudyLerp : MonoBehaviour
    {
        // # 보간? (Interpolation)
        // - 정해진 지점들 사이의 중간 과정을 만들어 내는 것
        // - 스터디를 위해 해당 챕터에서는 선형보간(Linear-Interpolation)
        //  만을 다룹니다. (Lerp = Linear-Interpolation의 약자)
        // - 등속도로 변화하는 가장 기본적인 보간 방식입니다.
        // - Unity에는 Mathf.Lerp, Vector3.Lerp, Color.Lerp 등등
        //  다양한 데이터 타입을 위한 보간 함수들이 내장되어 있습니다.

        // # 선형 보간
        // - 가장 기본적이고 널리 쓰이는 보간방식으로 "선형"이라는
        //  이름처럼 변화의 속도가 "일정한 등속 운동" 구현 합니다.
        // - 곡선을 이용해서 순간적인 속도 변화를 줄 수도 있습니다.
        // - 공식 : 결과 = 시작값 + (끝값 - 시작값) * t(진행시간)

        public bool isRun = false;
        public float speed = 1.0f;

        public Vector3 goalPosition;
        private Vector3 startPosition;

        public float goalTime = 2.0f;
        private float currentTime = 0.0f;

        private void Start()
        {
            startPosition = transform.position;
            goalPosition = transform.position + goalPosition;
        }

        private void Update()
        {
            // 원리에 대한 내용
            if (isRun)
            {
                // startPosition에서 goalPosition까지 보간을 이용해
                // 움직이는 코드를 작성해봅시다
                // - 공식 : 결과 = 시작값 + (끝값 - 시작값) * t(진행시간)
                currentTime += Time.deltaTime;

                float progress = currentTime / goalTime;
                //Vector3 currentPosition =
                //    startPosition + (goalPosition - startPosition) * progress;

                Vector3 currentPosition =
                    Vector3.Lerp(startPosition, goalPosition, progress);

                transform.position = currentPosition;

                if (currentTime > goalTime)
                {
                    currentTime = 0.0f;

                    Vector3 temp = startPosition;
                    startPosition = goalPosition;
                    goalPosition = temp;
                }
            }
        }
    }
}