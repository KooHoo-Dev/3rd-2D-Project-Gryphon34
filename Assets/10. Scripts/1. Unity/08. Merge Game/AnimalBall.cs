using Unity.VisualScripting;
using UnityEngine;

namespace Study.MergeGame
{
    public class AnimalBall : MonoBehaviour
    {
        public int level = 1;

        // c#의 프로퍼티를 이용해서 외부에서는 조회가능하지만
        // 변경에 대한 책임은 나만 가질 수 있는
        // 안전한 변수를 만들어줌
        public bool HasFirstContact { get; private set; } = false;

        public bool IsMerged { get; set; } = false;

        private Rigidbody2D rBody;
        private Collider2D col;

        private void Awake()
        {
            rBody = GetComponent<Rigidbody2D>();
            col=GetComponent<Collider2D>();
            //  gravityScale은 중력의 영향을 얼마나 받는지 결정하는 변수입니다
            rBody.gravityScale = 0.0f;
            rBody.bodyType = RigidbodyType2D.Kinematic;
            col.enabled = false;
        }

        public void Drop()
        {
            col.enabled = true;
            rBody.bodyType = RigidbodyType2D.Dynamic;
            rBody.gravityScale = 1.0f;

            // transform.SetParent()
            // : 부모자식 관계를 형성해주는 함수 입니다. null 입력시
            //  부모 transform이 없는 월드 최상위 개체가 됩니다
            transform.SetParent(null);
        }

        // OnCollision 시리즈
        // : OnTrigger와 함께 사용되는 물리 감지 이벤트 함수 입니다
        // rigidbody를 통해서 감지 할 수 있습니다

        // OnCollisionEnter2D
        // : Trigger가 아닌 Collider와 물리적 충돌 했을때 호출되는 이벤트 함수입니다

        // OnCollisionStay2D
        // : 물리적 충돌이 유지되고 있을때 호출되는 이벤트 함수 입니다

        // OnCollisionExit2D
        // : 물리적 충돌이 종료 되었을때 호출되는 이벤트 함수 입니다


        private void OnCollisionEnter2D(Collision2D collision)
        {
            HasFirstContact = true;

            // 두 볼이 병합되는 과정은
            // 1. 접촉이 일어난다
            // 2. 두 개체가 사라진다
            // 3. 상위개체가 그 중앙에 생긴다

            // 두 개체가 접촉해서 병합요청이 두 번 호출 될 수 있으므로,
            // 한쪽에 bool 변수를 넣어서 중복호출 되지 않도록 해줍니다.
            if (IsMerged) return;

            // 충돌 정보에서 나와 충돌한 gameObject에서 AnimalBall 컴포넌트를
            // 가져와 봅니다
            AnimalBall contactBall = collision.gameObject.GetComponent<AnimalBall>();
            
            // 벽에 부딪혀서 충돌한 개체에 AnimallBall 컴포넌트가 없다면
            if (contactBall == null) return;

            if (level != contactBall.level) return;

            MergeGameManager.Instance.MergeBall(this, contactBall);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (HasFirstContact == false) return;
            if (HasFirstContact&&collision.CompareTag("Finish"))
            {
                MergeGameManager.Instance.GameOver();
            }
        }
    }
}
