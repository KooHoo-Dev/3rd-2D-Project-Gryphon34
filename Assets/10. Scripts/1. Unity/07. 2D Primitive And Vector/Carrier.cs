using UnityEngine;
using System.Collections.Generic;

namespace Study.PrimitiveAndVector
{
    // # Carrier 설계 포인트
    // - 이 컴포넌트는 "무엇이 자기를 움직이는 지" 신경 쓰지 않습니다.
    // - 자기 위/안에 (특정 영역 = Sensor) 들어와 있는 객체를,
    // 자기 자신이 움직인 만큼 함께 움직이게 합니다
    // - 다른 스크립트가 움직여도 되고, 플레이어가 끌어도 됨

    // # 구현 방식
    // - 매 FixedUpdate에서 자기 Rigidbody의 poisition의 변화량(delta)를
    // 직접 계산해서 영역 안의 객체들에게 전달합니다.
    // - 어떻게 객체들을 감지할 것인가?
    // - 어떻게 변화량을 전달할 것인가?

    [DefaultExecutionOrder(-100)] // 실행 순서를 조절하는 키워드 입니다
    public class Carrier : MonoBehaviour
    {
        [Header("Settings")]
        public LayerMask interactionLayer;



        private Rigidbody2D rBody;
        private Vector3 prevPosition; // Vector2 => Vector3,  current => prev 로 수정
        private HashSet<Rigidbody2D> targets = new HashSet<Rigidbody2D>();

        private void Awake()
        {
            rBody = GetComponent<Rigidbody2D>();
            prevPosition = rBody.position;
        }

        private void FixedUpdate()
        {
            Vector3 delta = Vector3.zero;

            // delta 계산
            Vector3 currentPosition = rBody.position;
            delta = currentPosition - prevPosition;
            // 현재위치에서 이전위치를 차감하면 = 현재 프레임에서 내가 이동한 량
            // = 현재 프레임의 운동량
            prevPosition = currentPosition; // 계산 끝나면 prevPosition을 갱신

            foreach(Rigidbody2D rb in targets)
            {
                TransferMove(rb, delta);
            }
        }

        /// <summary>
        /// 운동량을 전달하는 함수. 한 개체씩 처리 합니다.
        /// </summary>
        private void TransferMove(Rigidbody2D rb, Vector3 delta)
        {
            // rb가 특수한 녀석일때 경우를 먼저 처리합시다.
            // 여기서 특수한 경우는 PlatformPlayer인 경우로 가정하겠습니다.
            // (PlatformPlayer는 자체적인 운동 방식을 가지고 있기 때문
            // = Kinematic 개체의 처리)

            PlatformPlayer platformPlayer = rb.GetComponent<PlatformPlayer>();
            if (platformPlayer != null)
            {
                platformPlayer.AddExternalMovement(delta);
                return;
            }

            // 기본적으로는 아래입니다.
            rb.position += new Vector2(delta.x, delta.y);
        }

        // 유효성 검사
        private bool IsValid(out Rigidbody2D body, Collider2D other)
        {
            body = null;
            if (interactionLayer.Contains(other.gameObject.layer) == false) return false;
            // interactionLayer가 other.gameObject.layer를 포함하고 있지 않다면 종료.
            body = other.gameObject.GetComponent<Rigidbody2D>();
            if (body == null) return false;
            // other.gameObject에서 Rigidbody2D 컴포넌트를 가져와보고, 없다면 종료.
            if (body == rBody) return false; // 자기자신이 검색될 수도 있어서 제외

            return true;
        }

        // other가 트리거 영역 안으로 처음 들어오는 순간 1회 호출 됩니다.
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsValid(out Rigidbody2D rb, other) == false) return;
            targets.Add(rb); // 운반 대상으로 설정
        }

        // other가 트리거 영역에서 빠져나가는 순간 1회 호출 됩니다.
        private void OnTriggerExit2D(Collider2D other)
        {
            if (IsValid(out Rigidbody2D rb, other) == false) return;
            targets.Remove(rb);
        }

        // other가 트리거 영역에 있을때 호출됩니다. (영역안에서 other가 움직일때 마다 호출됨)
        private void OnTriggerStay2D(Collider2D other)
        {

        }

    }
}
