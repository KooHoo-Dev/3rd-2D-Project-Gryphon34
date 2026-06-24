using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Study.PrimitiveAndVector
{
    public class CapsulePlayer1 : MonoBehaviour
    {
        // 캡슐 플레이어
        // 1. 화살표(좌,우)를 이용한 이동 및 표현
        // 2. Space버튼 이용한 점프
        // 3. Platform이라는 지형 위에서 움직여야 합니다.

        public enum State
        {
            Idle = 0,   // 대기 상태
            Left,       // 왼쪽으로 가는 상태
            Right       // 오른쪽으로 가는 상태
        }

        public GameObject[] SunGlasses;
        private State currentState = State.Idle;
        public float speed = 2.0f;
        private float platformMoveY = 0.0f;

        private Rigidbody2D rBody;
        private Collider2D col;
        //public float jumpPower = 100;

        private void Awake()
        {
            rBody = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();

            // 혹시 모르니 Awake에서 Kinematic으로 초기화 해줍니다
            // RigidbodyType 중 Kinematic은 "물리 엔진이 마음대로 못 건드리는 몸"
            // 오직 우리가 시키는 대로만 움직입니다
            // => 운동을 100% 직접 제어하기 위함.
            rBody.bodyType = RigidbodyType2D.Kinematic;
        }

        private bool jumpRequested = false;

        private void Update()
        {
            // FixedUpdate는 매프레임마다 실행되는게 아니라
            // 특정 시간마다 호출되어서, 프레임이 패싱 될 수 있습니다.
            // Update 함수에서 jumpRequested 우편함에 요청이 있었음을 기로가고
            // FixedUpdate에서 호출이 되는 ApplyJump 함수에서 
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

            // 이번 프레임에 움직이고 싶은 이동량
            Vector3 desireMove =
                new Vector3(horizontal * speed, verticalVelocity) * Time.fixedDeltaTime;
            desireMove.y += platformMoveY;
            // verticalVelocity에 Time.fixedDeltaTime 두번 적용이 되는데, 게임적 허용이라고
            // 봐주십쇼

            // 실제로 보정된 이동량 : 실제로 움직이게 될 이동량
            Vector3 moveVector = Vector3.zero;
            moveVector.x = ResolveAxisMovement(desireMove.x, Vector3.right);
            moveVector.y = ResolveAxisMovement(desireMove.y, Vector3.up);

            // 어딘가에 수직 움직임이 막혔다는 이야기
            if (moveVector.y != desireMove.y) verticalVelocity = 0.0f;

            // 마지막에 rigidbody에게 해당 위치로 이동하라고 명령 합니다
            rBody.MovePosition(transform.position + moveVector);
        }

        private void SetSunGlassState(State state)
        {
            if (currentState == state) return;
            // 상태가 전환될때만 아래 로직이 실행되도록
            // 예외처리를 합니다.

            SunGlasses[(int)currentState].SetActive(false);
            SunGlasses[(int)state].SetActive(true);
            currentState = state;
        }

        private void Move(Vector3 dir)
        {
            //transform.Translate(dir * speed * Time.deltaTime);
            //transform.position += (dir * (speed * Time.deltaTime));
            // Vector3             Vector3 * (float: distance)

            // 이번 프레임에 움직일 벡터의 크기 : 이번 프레임 이동량
            Vector3 moveVector = dir * (speed * Time.fixedDeltaTime);

            // 내 위치와 이동량을 더해줍니다
            rBody.MovePosition(transform.position + moveVector);
        }



        // 1. 중력을 구현
        // 2. 지형에 안착시키기
        // 3. 점프를 구현하기
        // 4. 끼임현상 없애기

        [Header("Settings")]
        public float gravity = -9.81f; // 중력 가속도(-9.81 = 현실세계)
        public float jumpPower = 8.0f; // 빨간줄 뜨면 안적어도됨
        public int maxJumpCount = 2;

        // 내부 상태 변수들
        private float verticalVelocity = 0.0f; // 실시간 수직 가속 운동 값
        private bool isGrounded = false; // 땅에 닿았는지 체크 하는 용도

        // # 중력 적용
        private void ApplyGravaity()
        {
            float groundStickSpeed = -2.0f;
            // 바닥에 있는 상태이면서 위로 올라가는 힘(점프)이 없다면
            if (isGrounded && verticalVelocity <= 0)
            {
                if (platformMoveY > 0)
                {
                    groundStickSpeed = 0f;
                }
                // 캐릭터가 바닥에 잘 붙어 줄수 있게 Damping값을 넣어 줍니다.
                verticalVelocity += groundStickSpeed;
                jumpCount = 0; //바닥에 붙어있으니 점프 카운트를 초기화 시킴
            }
            else // 공중에 있는 상태
            {
                // 물리엔진에 영향을 주는 코드라서 Time.fixedDeltaTime을 사용합니다.
                // 속도 = 속도 + 가속도 * 시간 (매 프레임마다 지속적으로 감소 합니다)
                verticalVelocity += gravity * Time.fixedDeltaTime;
            }
        }

        // 물리에도, 스크롤에서 사용하던것처럼 특정 계층만 모여있는 Layer 개념이 있습니다
        // 이번에 사용하는 Layer개념은 지형만 모아놓은 Ground Layer입니다.
        public LayerMask collisionLayer;

        // 한축의 이동을 해결하는 함수 (수평과 수직 공용)
        // move : 그 축으로 이동하려는 거리(부호 있음)
        // axis : 방향(Right, Up을 기준으로 동작함)
        // 반환 : 벽/바닥/천장 등에 막히면 줄어든 값을, 안 막히면 그대로의 이동량
        private float ResolveAxisMovement(float move, Vector3 axis)
        {
            if (move == 0.0f) return 0.0f;

            // 부호에 따라 실제로 검사할 방향을 정합니다
            Vector3 dir = move > 0.0f ? axis : -axis;
            float distance = Mathf.Abs(move);
            // Mathf.Abs(값) : 전달된 값을 절대값으로 반환하는 함수입니다.

            // Physics 검사를 진행합니다.
            // BoxCast를 위해서는 Box가 필요한데, 현재 캐릭터는 Capsule Collider를
            // 갖고 있습니다. AABB 검사를 이용하기 위해 임의의 박스를 생성합니다.
            // 우리는 이 Box를 Bounds 부릅니다.

            // 콜라이더보다 살짝 작은 박스를 그 방향으로 쏴 봅니다.
            Bounds box = GetCastBox();
            RaycastHit2D hit = Physics2D.BoxCast(
                box.center, box.size, 0.0f, dir, distance, collisionLayer);

            // 쏴보니까, 아무것도 없다면 원하는 만큼 이동해라
            if (hit.collider == null) return move;

            // 무언가가 있다면, 충돌 지점보다 skinWidth만큼 앞에서 멈춥니다
            float allowed = Mathf.Max(0, hit.distance - skinWidth);

            return Mathf.Sign(move) * allowed;
            // Mathf.Sign(값) 함수는 전달된 값의 부호만을 반환 합니다
        }

        // 캐릭터의 주위에 박스를 만들기위해 보정 용도 사용하는 변수
        public float skinWidth = 0.02f;

        // CastBox를 하기위한 Bounds를 반환하는 함수입니다.
        private Bounds GetCastBox()
        {
            Bounds box = col.bounds; //캡슐콜라이더도 bounds를 갖고 있습니다.

            // 박스 크기를 사방으로 조금씩 줄여줍니다.
            Vector3 size = box.size;
            size.x -= skinWidth * 2;
            size.y -= skinWidth * 2;
            box.size = size;

            return box;
        }
        private int jumpCount = 0;

        private void ApplyJump()
        {
            // 점프 하는 순간 y속도(수직 속도)를 "확" 끌어 올립니다.
            // 이후 ApplyGravity가 매 프레임 이 속도를 깎아서 포물선을 만들도록
            // 유도합니다.
            if (jumpRequested == false) return;
            jumpRequested = false;

            if (jumpCount >= maxJumpCount) return;

            verticalVelocity = jumpPower;
            jumpCount++;
            isGrounded = false; // 바닥에서 떨어졌음을 설정합니다.
        }

        // 바닥 검사 길이. 짧으면 짧을수록 디테일 해짐
        public float groundCheckDistance = 0.5f;
        // 바닥 판정
        // 캐스트 박스를 발밑으로 짧게 쏴서 닿는게 있으면 바닥으로 봅니다.
        // 발 밑으로만 쏴야함
        // ! 팁 : 이 방식을 응용해서 벽 충돌을 검사 가능(벽점프)
        private bool CheckGrounded()
        {
            Bounds box = GetCastBox();
            RaycastHit2D hit = Physics2D.BoxCast(box.center, box.size, 0.0f, Vector2.down, collisionLayer);

            platformMoveY = 0.0f;
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Platform"))
                {
                    StudyLerp1 hitPlatform=hit.collider.GetComponent<StudyLerp1>();
                    platformMoveY = hitPlatform.currentMovedelta.y;
                }
                return true;
            }
            return false;
        }

        #region ResolveAxisMovement 시각화 코드

        //[Header("캐스트 시각화 설정")]
        //// 시각화 표시 on/off
        //public bool showCastGizmos = true;

        //// 시각화에서 박스를 쏴 볼 거리 (보기 좋게, 실제 이동 거리와는 무관)
        //public float castPreviewDistance = 0.5f;

        //// 선/박스를 그릴 때 쓰는 1x1 흰색 텍스처
        //private static Texture2D whiteTexture;

        //// # OnGUI : 매 프레임 화면(게임 뷰)에 GUI를 그리는 이벤트 함수
        //private void OnGUI()
        //{
        //    if (showCastGizmos == false) return;

        //    Camera cam = Camera.main;
        //    if (cam == null || col == null) return;

        //    // 캐스트에 쓸 박스(콜라이더보다 살짝 작게)를 여기서 직접 계산합니다.
        //    Bounds bounds = col.bounds;
        //    Vector2 boxCenter = bounds.center;
        //    Vector2 boxSize = (Vector2)bounds.size - Vector2.one * (skinWidth * 2.0f);

        //    // 상/하/좌/우 네 방향으로 박스를 쏴 보고 그 결과를 그립니다.
        //    DrawBoxCast(cam, boxCenter, boxSize, Vector2.up);
        //    DrawBoxCast(cam, boxCenter, boxSize, Vector2.down);
        //    DrawBoxCast(cam, boxCenter, boxSize, Vector2.left);
        //    DrawBoxCast(cam, boxCenter, boxSize, Vector2.right);
        //}

        //// 한 방향으로 BoxCast를 직접 수행하고, 그 모양을 박스로 그립니다.
        //private void DrawBoxCast(Camera cam, Vector2 center, Vector2 size, Vector2 dir)
        //{
        //    RaycastHit2D hit = Physics2D.BoxCast(
        //        center, size, 0.0f, dir, castPreviewDistance, collisionLayer);

        //    Rect startRect = WorldBoxToGuiRect(cam, center, size);
        //    Rect endRect = WorldBoxToGuiRect(cam, center + dir * castPreviewDistance, size);

        //    // 검사 영역(시작~끝 전체) : 노란색
        //    DrawRectOutline(Encapsulate(startRect, endRect), 2.0f, Color.yellow);
        //    // 캐스트 시작 박스 : 흰색
        //    DrawRectOutline(startRect, 2.0f, Color.white);

        //    if (hit.collider != null)
        //    {
        //        // 무언가 걸렸으면 걸린 자리에 빨간 박스
        //        Rect hitRect = WorldBoxToGuiRect(cam, center + dir * hit.distance, size);
        //        DrawRectOutline(hitRect, 2.0f, Color.red);
        //    }
        //    else
        //    {
        //        // 아무것도 없으면 끝까지 간 자리에 초록 박스
        //        DrawRectOutline(endRect, 2.0f, Color.green);
        //    }
        //}

        //// 월드 공간의 축정렬 박스(center, size)를 GUI 좌표 Rect로 변환합니다.
        //private Rect WorldBoxToGuiRect(Camera cam, Vector2 center, Vector2 size)
        //{
        //    float z = transform.position.z;
        //    Vector3 worldMin = new Vector3(center.x - size.x * 0.5f, center.y - size.y * 0.5f, z);
        //    Vector3 worldMax = new Vector3(center.x + size.x * 0.5f, center.y + size.y * 0.5f, z);

        //    // 월드 좌표 -> 스크린 좌표 (스크린은 좌하단이 원점)
        //    Vector3 a = cam.WorldToScreenPoint(worldMin);
        //    Vector3 b = cam.WorldToScreenPoint(worldMax);

        //    float left = Mathf.Min(a.x, b.x);
        //    float right = Mathf.Max(a.x, b.x);
        //    // GUI 좌표는 좌상단이 원점이고 y가 아래로 증가 -> y를 뒤집어 줍니다.
        //    float top = Screen.height - Mathf.Max(a.y, b.y);
        //    float bottom = Screen.height - Mathf.Min(a.y, b.y);

        //    return new Rect(left, top, right - left, bottom - top);
        //}

        //// 두 Rect를 모두 감싸는 가장 작은 Rect를 만듭니다.
        //private Rect Encapsulate(Rect a, Rect b)
        //{
        //    return Rect.MinMaxRect(
        //        Mathf.Min(a.xMin, b.xMin), Mathf.Min(a.yMin, b.yMin),
        //        Mathf.Max(a.xMax, b.xMax), Mathf.Max(a.yMax, b.yMax));
        //}

        //// 사각형 "테두리"를 얇은 박스 4개(위/아래/왼/오른)로 그립니다.
        //private void DrawRectOutline(Rect r, float thickness, Color color)
        //{
        //    if (whiteTexture == null)
        //    {
        //        whiteTexture = new Texture2D(1, 1);
        //        whiteTexture.SetPixel(0, 0, Color.white);
        //        whiteTexture.Apply();
        //    }

        //    Color prev = GUI.color;
        //    GUI.color = color;

        //    GUI.DrawTexture(new Rect(r.xMin, r.yMin, r.width, thickness), whiteTexture);              // 위
        //    GUI.DrawTexture(new Rect(r.xMin, r.yMax - thickness, r.width, thickness), whiteTexture);  // 아래
        //    GUI.DrawTexture(new Rect(r.xMin, r.yMin, thickness, r.height), whiteTexture);             // 왼쪽
        //    GUI.DrawTexture(new Rect(r.xMax - thickness, r.yMin, thickness, r.height), whiteTexture); // 오른쪽

        //    GUI.color = prev;
        //}


        #endregion

    }
}