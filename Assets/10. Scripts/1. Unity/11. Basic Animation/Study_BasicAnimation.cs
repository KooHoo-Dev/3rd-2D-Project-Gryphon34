using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Study_BasicAnimation
{
    public class Study_BasicAnimation : MonoBehaviour
    {
        // # Animator?
        // - 각종 애니메이션 클립들을 상태에 맞게 제어할 수 있는 기능을
        //   제공하는 컴포넌트 입니다.
        // - 상태에 따라 애니메이션을 제어하는 것이 포인트.
        // - 상태끼리의 전환(Transition)을 이용해 애니메이션의 순환 구조를
        //   만들어 낼 수 있습니다.
        // - 2D뿐만 아니라 3D에서도 사용합니다.

        private Animator animator;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            animator=GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            /* SetBoll 사용
            bool isWalk = SimpleInput.GetKey(Key.RightArrow) || SimpleInput.GetKey(Key.LeftArrow);
            bool isRun = SimpleInput.GetKey(Key.LeftShift);

            animator.SetBool("IsWalk", isWalk);
            animator.SetBool("IsRun", isRun);
            */

            StudyAnimator();
        }

        // # Animator의 주요 필드와 메서드
        // .SetBool(), .GetBool()
        // : bool 파라미터를 제어합니다. 색인(id or string)과 bool 값을 전달합니다.

        // .SetFloat(), .GetFloat()
        // : float 파라미터를 제어합니다. 색인 (id or string)과 float값을 전달합니다.

        // . SetInteger(), .GetInteger()
        // : int 파라미터를 제어합니다. 색인 (id or string)과 Integer값을 전달합니다.

        // .SetTrigger(), .ResetTrigger()
        // : 트리거 파라미터를 발생시킵니다. 색인 (id or string)만을 전달합니다.
        // ResetTrigger()는 해당 트리거의 입력 상태를 원상복귀 시킵니다. (없는 상태로)

        // .GetCurrentAnimatorStateInfo(0) : 현재 재생중인 State 정보를 가져옵니다.

        public float speed = 1.0f;

        private void StudyAnimator()
        {
            float moveDirection = SimpleInput.GetAxisHorizontalRaw();

            // 캐릭터의 방향 전환과 애니메이션 적용 값을 같이 구현합니다.
            // 이동 입력이 있는지 판별. 0보다 크다면 이동 입력이 있는 것
            float absMoveDirection = Mathf.Abs(moveDirection);
            animator.SetFloat("Movement", absMoveDirection);

            /* 이렇게도 가능
            if (absMoveDirection > 0)
            {
                Vector3 currentScale = transform.localScale;
                currentScale.x = Mathf.Abs(currentScale.x)*moveDirection;
                transform.localScale = currentScale;
            */

            // 아래처럼 표현하는걸 추천
            if (absMoveDirection > 0) spriteRenderer.flipX = moveDirection < 0.0f;

            // 달리기 까지 포함한 이동 연산
            bool isRun = SimpleInput.GetKey(Key.LeftShift);
            animator.SetBool("IsRun", isRun);

            float applySpeed = speed * (isRun ? 2.5f : 1.0f);

            transform.Translate(Vector3.right * moveDirection * applySpeed * Time.deltaTime);

            }
        }
    }

