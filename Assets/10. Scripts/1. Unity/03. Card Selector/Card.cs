using UnityEngine;
using UnityEngine.InputSystem;

namespace Study.CardSelector
{
    public class Card : MonoBehaviour
    {
        public enum Number
        {
            Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Ace,
        }

        public enum State
        {
            Back,
            Front,
        }

        [Header("공용 데이터")]
        public Sprite backImage;
        public Sprite[] numberImages;

        [Header("My Data")]
        public SpriteRenderer cardRenderer;
        public Number number;
        public State state;

        void Awake()
        {
            // gameObject 멤버 변수는 특별해서 컴포넌트 객체가
            // 부착된 GameObject에 접근할 수 있습니다.
            cardRenderer = gameObject.GetComponent<SpriteRenderer>();
            // GetComponent<가져올타입>();
            // 부착된 게임오브젝트에서 {가져올타입}의 객체를 가져와서 반환합니다.
            // 만약 부착되어있지 않다면 null을 반환합니다

            SetState(State.Back);
        }

        /// <summary>
        /// 카드의 출력 상태를 설정합니다.
        /// </summary>
        public void SetState(State state)
        {
            // 나의 현재 상태(myState)를 갱신한다
            // cardRenderer의 이미지를 알맞게 바꿔준다.

            if (state == State.Back)    cardRenderer.sprite = backImage;
            else                        cardRenderer.sprite = numberImages[(int)number];

            this.state = state;
        }

        /// <summary>
        /// 카드의 상태를 반전합니다. Back => Front, Front => Back
        /// </summary>
        public void Flip()
        {
            if (state == State.Back)  SetState(State.Front);
            else                        SetState(State.Back);
        }
    }
}

