using Study.Utilities;
using UnityEngine;

namespace Study.MiniFarm
{
    // BlendTree?
    // : Animator의 State 하나 안에서 여러 애니메이션 클립을
    //  파라미터 값에 따라 연속적으로 섞어주는(blend) 구조.
    //  State간 Transition이 "A -> B" 라면, 블렌드 트리는
    //  A와 B를 3:7로 섞어서 재생해줘! 에 가깝습니다.
    //  - 걷기 -> 달리기 처럼 연속적인 변화에 사용합니다
    //  - 클립이 많아져도 State는 한개 = State 폭발이 없어 집니다.
    
    // 만드는 방법
    // : Animator Window에서 우클릭 -> Create State
    // -> From New Blend Tree
    // 진입 방법
    // : Animator Window에서 해당 블렌드 트리 더블 클릭
    
    // 블렌드 타입 5종
    // # 1D
    // : 파라미터는 float 1개. 주로 Idle -> Walk -> Run 등의 속도만 제어하는
    //  사이드뷰 캐릭터 애니메이션에 사용함
    // # 2D Simple Directional
    // : 파라미터가 float 2개. 방향별 클립이 1개씩 있을 때. 탑뷰, 쿼터뷰, 등각투영(아이소메트릭)
    //  뷰 등의 위에서 바라보는 시점의 캐릭터 애니메이션 표현에 사용함.
    // # 2D Freeform Directional
    // : 파라미터가 float 2개. 같은 방향에 속도가 다른 클립이 여럿일경우 (걷기/달리기 8방향 등)
    //  주로 3D 캐릭터의 이동 표현에 사용합니다
    // # 2D Freeform Cartesian
    // : 파라미터가 float 2개. 방향이 없는 축 조합등에 사용. 2D에서는 거의 안씀. 속도x회전각 등의
    //  표현에 사용함
    // # Direct 
    // : 클립마다 float 1개가 매핑됨. 블렌드 셰이프나 캐릭터의 표정 연출등에 사용함
    
    
    public class Study_BlendTree : MonoBehaviour
    {
        private Animator anim;
        private Vector2 currentDir;
        
        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        private void Update()
        {
            var input = SimpleInput.GetMoveAxisRaw();
            if (input == Vector2.zero) return;
            if (input == currentDir) return;
            currentDir = input;
            
            anim.SetFloat("DirX", currentDir.x);
            anim.SetFloat("DirY", currentDir.y);

            if (Mathf.Abs(currentDir.x) > Mathf.Epsilon)
            {
                transform.localScale = new Vector3(Mathf.Sign(currentDir.x), 1, 1);
            }
        
        }
    }
}