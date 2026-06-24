using UnityEngine;

namespace Study.PrimitiveAndVector
{
    [DefaultExecutionOrder(-200)] // 실행 순서를 조절하는 키워드 입니다
    public class Mover : MonoBehaviour
    {
        public float speed = 1.0f;

        public Vector3 goalPosition;
        private Vector3 startPosition;

        public float goalTime = 2.0f;
        private float currentTime = 0.0f;

        private Rigidbody2D rBody;

        private void Awake()
        {
            rBody = GetComponent<Rigidbody2D>();
            rBody.bodyType = RigidbodyType2D.Kinematic;
            // Kinematic : 코드로 운동을 정의할때 사용함

            startPosition = transform.position;
            goalPosition = transform.position + goalPosition;
        }

        private void FixedUpdate()
        {
            currentTime += Time.fixedDeltaTime;

            float progress = currentTime / goalTime;

            Vector3 currentPosition =
                Vector3.Lerp(startPosition, goalPosition, progress);

            //transform.position = currentPosition; // 이걸 지우고 아래의 코드로
            rBody.MovePosition(currentPosition);

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