using UnityEngine;
using UnityEngine.InputSystem;

namespace Study.PrimitiveAndVector
{
    public class PlatformPlayer : MonoBehaviour
    {
        public enum State
        {
            Idle = 0,   // 대기 상태
            Left,       // 왼쪽으로 가는 상태
            Right       // 오른쪽으로 가는 상태
        }

        public GameObject[] SunGlasses;
        private State currentState = State.Idle;
        public float speed = 2.0f;

        private Rigidbody2D rBody;
        private Collider2D col;

        [Header("Settings")]
        public float gravity = -9.81f; // 중력 가속도(-9.81 = 현실세계)
        public float jumpPower = 8.0f; // 빨간줄 뜨면 안적어도됨
        public int maxJumpCount = 2;
        public float groundCheckDistance = 0.5f;  // 바닥 검사 길이. 짧으면 짧을수록 디테일해짐
        public float skinWidth = 0.02f; // 캐릭터의 주위에 박스를 만들기위해 보정 용도 사용하는 변수

        public LayerMask collisionLayer;

        private float verticalVelocity = 0.0f; // 실시간 수직 가속 운동 값
        private bool isGrounded = false; // 땅에 닿았는지 체크 하는 용도
        private int jumpCount = 0;
        private bool jumpRequested = false;

        private Vector3 externalDelta;

        private void Awake()
        {
            rBody = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            rBody.bodyType = RigidbodyType2D.Kinematic;
        }

        private void Update()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                jumpRequested = true;
            }
        }

        private void FixedUpdate()
        {
            isGrounded = CheckGrounded();

            // 수평의 이동량을 결정하기 위한 변수
            float horizontal = 0.0f;

            if (Keyboard.current.leftArrowKey.isPressed)
            {
                SetSunGlassState(State.Left);
                horizontal = -1.0f;
            }
            else if (Keyboard.current.rightArrowKey.isPressed)
            {
                SetSunGlassState(State.Right);
                horizontal = 1.0f;
            }
            else
            {
                SetSunGlassState(State.Idle);
                horizontal = 0.0f;
            }

            ApplyGravaity();
            ApplyJump();

            Vector3 desireMove =
                new Vector3(horizontal * speed, verticalVelocity) * Time.fixedDeltaTime
                + externalDelta;

            externalDelta = Vector3.zero;

            Vector3 moveVector = Vector3.zero;
            moveVector.x = ResolveAxisMovement(desireMove.x, Vector3.right);
            moveVector.y = ResolveAxisMovement(desireMove.y, Vector3.up);

            if (moveVector.y != desireMove.y) verticalVelocity = 0.0f;

            rBody.MovePosition(transform.position + moveVector);
        }

        public void AddExternalMovement(Vector3 delta)
        {
            externalDelta += delta;
        }


        private void SetSunGlassState(State state)
        {
            if (currentState == state) return;

            SunGlasses[(int)currentState].SetActive(false);
            SunGlasses[(int)state].SetActive(true);
            currentState = state;
        }

        // # 중력 적용
        private void ApplyGravaity()
        {
            const float groundStickSpeed = -2.0f;

            if (isGrounded && verticalVelocity <= 0)
            {
                verticalVelocity =
                    (externalDelta == Vector3.zero) ? groundStickSpeed : 0.0f;

                //verticalVelocity += groundStickSpeed;
                jumpCount = 0; // 바닥에 붙어있으니 점프 카운트를 초기화 시킴
            }
            else // 공중에 있는 상태
            {
                verticalVelocity += gravity * Time.fixedDeltaTime;
            }
        }

        private float ResolveAxisMovement(float move, Vector3 axis)
        {
            if (move == 0.0f) return 0.0f;

            Vector3 dir = move > 0.0f ? axis : -axis;
            float distance = Mathf.Abs(move);

            Bounds box = GetCastBox();
            RaycastHit2D hit = Physics2D.BoxCast(
                box.center, box.size, 0.0f, dir, distance, collisionLayer);

            if (hit.collider == null) return move;
            if (hit.distance == 0.0f) return move; // 박혀있을때 처리

            float allowed = Mathf.Max(0, hit.distance - skinWidth);
            return Mathf.Sign(move) * allowed;
        }

        private Bounds GetCastBox()
        {
            Bounds box = col.bounds; //캡슐콜라이더도 bounds를 갖고 있습니다.

            Vector3 size = box.size;
            size.x -= skinWidth * 2;
            size.y -= skinWidth * 2;
            box.size = size;

            return box;
        }

        private void ApplyJump()
        {
            if (jumpRequested == false) return;
            jumpRequested = false; //우편함을 비웁니다.

            // 점프 횟수 제어
            if (jumpCount >= maxJumpCount) return;

            verticalVelocity = jumpPower;
            jumpCount++;
            isGrounded = false; // 바닥에서 떨어졌음을 설정합니다.
        }

        private bool CheckGrounded()
        {
            Bounds box = GetCastBox();
            RaycastHit2D hit = Physics2D.BoxCast(
                box.center, box.size, 0.0f, Vector2.down, groundCheckDistance, collisionLayer);

            bool result = (hit.collider != null);
            return result;
        }



        #region ResolveAxisMovement 시각화 코드

        [Header("캐스트 시각화 설정")]
        // 시각화 표시 on/off
        public bool showCastGizmos = true;

        // 시각화에서 박스를 쏴 볼 거리 (보기 좋게, 실제 이동 거리와는 무관)
        public float castPreviewDistance = 0.5f;

        // 선/박스를 그릴 때 쓰는 1x1 흰색 텍스처
        private static Texture2D whiteTexture;

        // # OnGUI : 매 프레임 화면(게임 뷰)에 GUI를 그리는 이벤트 함수
        private void OnGUI()
        {
            if (showCastGizmos == false) return;

            Camera cam = Camera.main;
            if (cam == null || col == null) return;

            // 캐스트에 쓸 박스(콜라이더보다 살짝 작게)를 여기서 직접 계산합니다.
            Bounds bounds = col.bounds;
            Vector2 boxCenter = bounds.center;
            Vector2 boxSize = (Vector2)bounds.size - Vector2.one * (skinWidth * 2.0f);

            // 상/하/좌/우 네 방향으로 박스를 쏴 보고 그 결과를 그립니다.
            DrawBoxCast(cam, boxCenter, boxSize, Vector2.up);
            DrawBoxCast(cam, boxCenter, boxSize, Vector2.down);
            DrawBoxCast(cam, boxCenter, boxSize, Vector2.left);
            DrawBoxCast(cam, boxCenter, boxSize, Vector2.right);
        }

        // 한 방향으로 BoxCast를 직접 수행하고, 그 모양을 박스로 그립니다.
        private void DrawBoxCast(Camera cam, Vector2 center, Vector2 size, Vector2 dir)
        {
            RaycastHit2D hit = Physics2D.BoxCast(
                center, size, 0.0f, dir, castPreviewDistance, collisionLayer);

            Rect startRect = WorldBoxToGuiRect(cam, center, size);
            Rect endRect = WorldBoxToGuiRect(cam, center + dir * castPreviewDistance, size);

            // 검사 영역(시작~끝 전체) : 노란색
            DrawRectOutline(Encapsulate(startRect, endRect), 2.0f, Color.yellow);
            // 캐스트 시작 박스 : 흰색
            DrawRectOutline(startRect, 2.0f, Color.white);

            if (hit.collider != null)
            {
                // 무언가 걸렸으면 걸린 자리에 빨간 박스
                Rect hitRect = WorldBoxToGuiRect(cam, center + dir * hit.distance, size);
                DrawRectOutline(hitRect, 2.0f, Color.red);
            }
            else
            {
                // 아무것도 없으면 끝까지 간 자리에 초록 박스
                DrawRectOutline(endRect, 2.0f, Color.green);
            }
        }

        // 월드 공간의 축정렬 박스(center, size)를 GUI 좌표 Rect로 변환합니다.
        private Rect WorldBoxToGuiRect(Camera cam, Vector2 center, Vector2 size)
        {
            float z = transform.position.z;
            Vector3 worldMin = new Vector3(center.x - size.x * 0.5f, center.y - size.y * 0.5f, z);
            Vector3 worldMax = new Vector3(center.x + size.x * 0.5f, center.y + size.y * 0.5f, z);

            // 월드 좌표 -> 스크린 좌표 (스크린은 좌하단이 원점)
            Vector3 a = cam.WorldToScreenPoint(worldMin);
            Vector3 b = cam.WorldToScreenPoint(worldMax);

            float left = Mathf.Min(a.x, b.x);
            float right = Mathf.Max(a.x, b.x);
            // GUI 좌표는 좌상단이 원점이고 y가 아래로 증가 -> y를 뒤집어 줍니다.
            float top = Screen.height - Mathf.Max(a.y, b.y);
            float bottom = Screen.height - Mathf.Min(a.y, b.y);

            return new Rect(left, top, right - left, bottom - top);
        }

        // 두 Rect를 모두 감싸는 가장 작은 Rect를 만듭니다.
        private Rect Encapsulate(Rect a, Rect b)
        {
            return Rect.MinMaxRect(
                Mathf.Min(a.xMin, b.xMin), Mathf.Min(a.yMin, b.yMin),
                Mathf.Max(a.xMax, b.xMax), Mathf.Max(a.yMax, b.yMax));
        }

        // 사각형 "테두리"를 얇은 박스 4개(위/아래/왼/오른)로 그립니다.
        private void DrawRectOutline(Rect r, float thickness, Color color)
        {
            if (whiteTexture == null)
            {
                whiteTexture = new Texture2D(1, 1);
                whiteTexture.SetPixel(0, 0, Color.white);
                whiteTexture.Apply();
            }

            Color prev = GUI.color;
            GUI.color = color;

            GUI.DrawTexture(new Rect(r.xMin, r.yMin, r.width, thickness), whiteTexture);              // 위
            GUI.DrawTexture(new Rect(r.xMin, r.yMax - thickness, r.width, thickness), whiteTexture);  // 아래
            GUI.DrawTexture(new Rect(r.xMin, r.yMin, thickness, r.height), whiteTexture);             // 왼쪽
            GUI.DrawTexture(new Rect(r.xMax - thickness, r.yMin, thickness, r.height), whiteTexture); // 오른쪽

            GUI.color = prev;
        }


        #endregion

    }
}