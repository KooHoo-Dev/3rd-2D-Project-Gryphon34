using UnityEngine;
using UnityEngine.InputSystem;


namespace Study.CardSelector
{
    public class CardSelector : MonoBehaviour
    {
        public Card[] cards;
        public UnityEngine.Transform cursor;
        public int currentIndex = 2;

        private void Update()
        {
            if(Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                // 왼쪽키를 누를 경우
                // 인덱스를 차감하고
                // 해당되는 카드의 x위치값을 가져와서
                // curor의 x값에 대입해준다

                // 이전 코드
                //currentIndex -= 1;
                //float cardX = cards[currentIndex].transform.position.x;
                //cursor.position = new Vector3(cardX, cursor.position.y);

                MoveCursor(true);
            }
            else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                // 이전 코드
                //currentIndex += 1;
                //float cardX = cards[currentIndex].transform.position.x;
                //cursor.position = new Vector3(cardX, cursor.position.y);

                MoveCursor(false);
            }

            if(Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                DeleteCard(cards[0], cards[1]);

                //SelectCard();
            }
        }

        private void MoveCursor(bool isLeft)
        {
            // 삼항연산자 (조건식) ? (값1) : (값2)
            // 조건식이 참일경우 값1을 반환합니다.
            // 조건식이 거짓일 경우 값2를 반환합니다.

            currentIndex += isLeft ? -1 : +1;

            // version1
            // 범위 밖으로 움직이지 못하게 하는 경우
            //if (currentIndex < 0) currentIndex = 0;
            //if (currentIndex >= cards.Length) currentIndex = cards.Length - 1;

            // Clamp(변환할 값, 최소값, 최대값)
            // : 변환할 값을 비교하여 최소값과 최대값 사이를 만족하는 값을 반환합니다
            //  변환할 값이 최소값보다 작다면 => 최소값이 반환이 됩니다.
            //  변활할 값이 최대값보다 크다면 => 최대값이 반환이 됩니다.
            
            // 아래코드 주석 해제
            //currentIndex = Mathf.Clamp(currentIndex, 0, cards.Length - 1);

            // version2
            // 최소값보다 작을 경우 최대값으로 바꿔버리기
            // 최대값보다 클 경우 최소값으로 바꿔버리기

            if (currentIndex < 0) currentIndex = cards.Length - 1;
            if (currentIndex >= cards.Length) currentIndex = 0;

            float cardX = cards[currentIndex].transform.position.x;
            cursor.position = new Vector3(cardX, cursor.position.y);
        }

        private void SelectCard()
        {
            cards[currentIndex].Flip();
        }

        /// <summary>
        /// a와 b가 동일한 카드(숫자가 같을 경우)일 경우 호출하세요
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private void DeleteCard(Card a, Card b)
        {
            Destroy(a.gameObject); // a 컴포넌트가 부착된 게임오브젝트를 삭제합니다
            Destroy(b.gameObject); // b 컴포넌트가 부착된 게임오브젝트를 삭제합니다
        }
    }
}


