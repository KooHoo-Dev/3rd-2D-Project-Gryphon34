using Study.CardSelector;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.GPUSort;


namespace Study.PairMatchingGame
{
    public class PairMatchingGame : MonoBehaviour
    {
        [Header("Ref Object")]
        public CardSelector cardSelector;
        public Card[]       board;
        public GameObject   clearObject;
        
        private int pairMatchingCount = 0;

        private void Awake()
        {
            int[] indexBuffer = new int[board.Length];
            for(int i = 0; i < indexBuffer.Length; ++i)
            {
                // board의 인덱스를 넣어준다
                indexBuffer[i] = i;
            }

            // 인덱스 버퍼를 섞어줍니다
            for (int i = indexBuffer.Length - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                int temp = indexBuffer[i];
                indexBuffer[i] = indexBuffer[j];
                indexBuffer[j] = temp;
            }

            for (int i = 0; i < board.Length; i+=2)
            {
                // Unity에서 랜덤을 사용하는 방법
                // .Net의 랜덤을 사용해도 되긴합니다만, 더 편한방법이 있습니다.

                int randNum = Random.Range((int)Card.Number.Two, (int)Card.Number.Ace + 1);
                // Random.Range(최소값, 최대값) 최소값~최대값 범위중에 임의의 수를 반환합니다
                // !! 주의 : 최대값은 해당 범위에 포함되지 않습니다.

                // 인덱스 버퍼를 두개씩 가지고와서 randNum을 대입해줍니다
                int indexA = indexBuffer[i];
                int indexB = indexBuffer[i+1];
                board[indexA].number = (Card.Number)randNum;
                board[indexB].number = (Card.Number)randNum;
            }


            cardSelector.SetBoard(board);
            clearObject.SetActive(false);
        }

        private void LateUpdate()
        {
            if(Keyboard.current.f10Key.wasPressedThisFrame)
            {
                for (int i = 0; i < board.Length; i++)
                {
                    board[i].Flip();
                }
            }

            if (cardSelector.WasSelectionCompleted)
            {
                Card[] selectedCard = cardSelector.GetSelectedCards();
                CheckPairMatching(selectedCard[0], selectedCard[1]);
            }
        }

        private void CheckPairMatching(Card a, Card b)
        {
            // 같으면 지우고
            if(a.number == b.number)
            {
                DeleteCard(a);
                DeleteCard(b);

                pairMatchingCount += 2; //같으면 페어매칭카운트를 증가시킴
                CheckEnd();
            }
            else // 다르면 뒤집어라
            {
                a.Flip();
                b.Flip();

                Debug.Log("두 카드가 다릅니다");
            }

            cardSelector.Clear();
        }

        private void DeleteCard(Card target)
        {
            // 선형탐색을 이용해서 target의 위치를 찾습니다
            for(int i = 0; i < board.Length; ++i)
            {
                // null인곳은 건너 뜁니다.
                if (board[i] == null) continue;

                // Equals(매개변수) 함수는 "==" 같다고 생각해주세요
                if (board[i].Equals(target)) 
                {
                    board[i] = null;            //먼저 배열에서 비워준 후
                    Destroy(target.gameObject); // Scene에서 삭제합니다
                }
            }
        }
        
        private void CheckEnd()
        {
            if (pairMatchingCount >= board.Length)
            {
                //게임이 끝났다
                clearObject.SetActive(true);

                // ~.SetActive(bool 매개변수)
                // 해당 게임오브젝트의 활성화/비활성화를 제어하는 함수입니다.
                // gameObject.SetActive(true); => 게임오브젝트가 활성화 됩니다.
                // gameObject.SetActive(false); => 게임오브젝트가 비 활성화 됩니다.
            }
        }
    }
}


