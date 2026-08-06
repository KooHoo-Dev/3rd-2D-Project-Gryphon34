using System;
using UnityEngine;

namespace Study.MiniFarm
{
    public enum FarmerAction
    {
        PickUp,
        Throwing,
        Hoe,
        Watering,
    }
    
    // 농부의 겉모습을 맡는다.
    // FarmerAnimator를 코드에서 다룰 수 있게 감싼 클래스입니다
    // 여기서 처리하지 않는 일은 아래와 같습니다.
    // - 움직이지 않는다 : 위치 변환이 없다
    // - 게임 규칙을 따지지 않는다 : "앞이 막혀있으니 못간다" 이런거 따지지 않는다
    // 이 컴포넌트는 "어느쪽을 바라보고, 걷는 모션을 재생하고, 무슨 동작을 재생하라"
    // 이런 명령만 받습니다.
    
    public class FarmerController : MonoBehaviour
    {
        private static readonly int PickUp = Animator.StringToHash("PickUp");
        private static readonly int Throwing = Animator.StringToHash("Throwing");
        private static readonly int Hoe = Animator.StringToHash("Hoe");
        private static readonly int Watering = Animator.StringToHash("Watering");
        private static readonly int IsWalk = Animator.StringToHash("IsWalk");
        private static readonly int DirX = Animator.StringToHash("DirX");
        private static readonly int DirY = Animator.StringToHash("DirY");

        private Animator animator;
        private SpriteRenderer spriteRenderer;

        // 마지막으로 바라본 방향.
        public Vector2Int Facing { get; private set; } = Vector2Int.down;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        
        // 바라볼 방향을 외부에서 입력해준다.
        // 대각선을 줘도 상하좌우 중 하나로 스냅될 수 있도록 한다.
        // 길이가 0이거나 0에 가까울 경우 방향을 바꾸지 않는다.
        // (블렌드 트리에 (0,0) 좌표의 클립이 없기 때문에 처리해 줘야함)
        public void Face(Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.0001f) return;
            
            // x축과 y축의 값을 비교해서 더 큰쪽으로 몰아준다(?)
            // : 대각선 값(0.7, 0.7)의 값이 들어오면 비교를 해서
            //  한쪽으로 몰아준다.
        
            Vector2Int snapped = (Mathf.Abs(direction.x) >= (Mathf.Abs(direction.y))
                    ? new Vector2Int((direction.x > 0 ? 1 : -1), 0)
                    : new Vector2Int(0, (direction.y > 0 ? 1 : -1)));
                    // (direction.x > 0 ? 1 : -1)
                    // : x가 0보다 크면 무조건 1로 본다. x가 0보다 작으면 무조건 -1로 본다
            
            // 마지막으로 바라본 방향과 snapped 같다면 return;
            if (snapped == Facing) return;

            Facing = snapped;
            ApplyFacing();
        }

        // 스프라이트 정면 처리에 사용하는 필드
        public bool artFacesRight = true;
        
        // animator에 실제로 적용을 하는 함수
        private void ApplyFacing()
        {
            animator.SetFloat(DirX, Facing.x);
            animator.SetFloat(DirY, Facing.y);

            if (Facing.x != 0)
            {
                spriteRenderer.flipX = artFacesRight ? 
                    (Facing.x < 0) : (Facing.x > 0);
            }
        }

        public void SetWalking(bool walking)
        {
            if(animator.GetBool(IsWalk) == walking) return;
            animator.SetBool(IsWalk, walking);
        }

        public void Play(FarmerAction action)
        {
            int animHash = action switch
            {
                FarmerAction.PickUp => PickUp,
                FarmerAction.Throwing => Throwing,
                FarmerAction.Hoe => Hoe,
                FarmerAction.Watering => Watering,
                _ => Hoe,
            };
            
            animator.SetTrigger(animHash);
        }
    }
}