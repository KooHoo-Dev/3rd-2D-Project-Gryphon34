using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Study.MergeGame
{
    public class MergeGameController : MonoBehaviour
    {
        // Scene안에 유저가 제어 할 수 있는 UFO에 부착되어있는 스크립트 입니다.
        // 마우스 클릭 상태에서 왼쪽, 오른쪽 드래그시 UFO가 움직입니다
        // 마우스 클릭을 떼는 순간, AnimalBall을 떨어뜨립니다

        [Header("Settings")]
        public float speed = 1.0f;
        public float minXPosition = -2.5f; // 좌측 최소값
        public float maxXPosition = 2.5f;  // 우측 최대값

        [Header("Ref")]
        public Transform spawnPoint;

        private AnimalBall currentBall = null; // 내부 상태 제어를 위한 빈 객체

        #region Unity Methods

        private void Start()
        {
            SpawnBall();
        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // 커서 제어 로직 (사라지게 하는 것)
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked; // 화면 중앙에 고정
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                // 커서 제어 로직 (나타나게 하는 것)
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined; // 화면 중앙에 고정
                DropBall();
            }
            else if (Mouse.current.leftButton.isPressed)
            {
                UpdateMovement();
            }
        }

        #endregion

        private void UpdateMovement()
        {
            // Mouse, Touch 등의 입력의 특징
            // - 2차원 좌표계를 입력받을 수 있음
            // - 이전 프레임과 현재 프레임의 변동량을 받을 수 있음 => delta
            // - 키 입력의 시작, 유지, 종료 등을 받을 수 있음

            // 한 프레임당 마우스 좌표 변화량을 가지고 옵니다
            Vector2 mouseDelta = Mouse.current.delta.value;

            // 게임내의 좌표계로 바꿔주면서 적용합니다
            Vector3 moveVector =
                new Vector3(mouseDelta.x, 0.0f, 0.0f) * speed * Time.deltaTime;

            Vector3 adjustPosition = transform.position + moveVector;
            adjustPosition.x =
                Mathf.Clamp(adjustPosition.x, minXPosition, maxXPosition);
            // x가 구산을 벗어나지 않게 하는 값

            transform.position = adjustPosition;
        }

        public MergeGameManager manager;

        private void SpawnBall()
        {
            // MergeGameManager에서 공을 가지고 와서
            // SpawnPoint에 생성을 합니다.

            AnimalBall spawnBall = MergeGameManager.Instance.GetNextBall();
            // MergeGameManager에서 가져온 프리펩을 이용해서 새로운 볼을 만들어준다
            // Instantiate(견본, Transform 매개변수)
            // : 매개변수로 전달된 Transform의 자식으로 복사본을 생성합니다.
            currentBall = Instantiate(spawnBall, spawnPoint);
            // 생성한 볼을 spawnPoint의 위치로 초기화 해준다
            currentBall.transform.localPosition = Vector3.zero; // 부모 좌표와 동일하게 만들어줌

        }

        private void DropBall()
        {
            if (currentBall == null) return;
            currentBall.Drop();
            StartCoroutine(SpawnDelayCoroutine(currentBall));
            currentBall = null;
        }

        private IEnumerator SpawnDelayCoroutine(AnimalBall dropBall)
        {
            while (true)
            {
                // 떨군 볼이 어딘가에 닿았다면
                if (dropBall.HasFirstContact) break;
                yield return null; // 한 프레임 대기, 지연 입니다.
            }

            SpawnBall();
        }
    }

}

