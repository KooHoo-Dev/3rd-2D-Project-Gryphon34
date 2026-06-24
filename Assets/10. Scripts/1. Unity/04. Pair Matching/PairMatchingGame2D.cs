using Study.CardSelector;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.GPUSort;


namespace Study.PairMatchingGame
{
    public class PairMatchingGame2D : MonoBehaviour
    {
        [Header("Ref Object")]
        public CardSelector2D cardSelector2D;
        public GameObject clearObject;

        private int pairMatchingCount = 0;

        public Card[,] board2D;

        // 이 부분은 인스펙터에 노출해서 수정 가능하도록 설계합니다
        public int columnCount = 5;
        public int rowCount = 4;


        private void Awake()
        {
            // T는 타입 매개변수 입니다. 타입(클래스, 컴포넌트등)을 넣으시면 됩니다.

            // .GetComponent<T>()
            // : 나의 transform을 기준으로 T 객체(컴포넌트)를 검색해서 반환합니다.
            // .GetComponentInChildren<T>()
            // : 나의 자식들을 기준으로 T 객체(컴포넌트)를 검색해서 반환합니다.

            // .GetComponents<T>()
            // : 나의 transform을 기준으로 T 객체(컴포넌트)들을 검색해서 배열로 반환합니다.
            // .GetComponentsInChildren<T>()
            // : 나의 자식들을 기준으로 T 객체(컴포넌트)들을 검색해서 배열로 반환합니다.

            Card[] myChildrenCards = GetComponentsInChildren<Card>();
            // 나를 기준으로 내 자식 GameObject에 존재하는 Card 컴포넌트 들을
            // 찾아서 myChildrenCards에 대입 해줘.

            Card[] cards = InitCardArray(myChildrenCards);

            // 아래부터는 2차원 배열 초기화 로직
            // cards를 2차원 배열로 쪼개봅시다. Column의 갯수와 Row의 갯수가 필요합니다.

            board2D = new Card[columnCount, rowCount];

            for (int i = 0; i < cards.Length; ++i)
            {
                int x = i % columnCount;
                int y = i / columnCount;
                board2D[x, y] = cards[i];
            }

            cardSelector2D.SetBoard(board2D);

            //cardSelector.SetBoard(board);
            clearObject.SetActive(false);

            // 매개변수로 전달된 Card 배열을 로직에 맞게 초기화 합니다.
            Card[] InitCardArray(Card[] cardArray)
            {
                int[] indexBuffer = new int[cardArray.Length];
                for (int i = 0; i < indexBuffer.Length; ++i) indexBuffer[i] = i;

                for (int i = indexBuffer.Length - 1; i > 0; i--)
                {
                    int j = Random.Range(0, i + 1);
                    int temp = indexBuffer[i];
                    indexBuffer[i] = indexBuffer[j];
                    indexBuffer[j] = temp;
                }

                for (int i = 0; i < cardArray.Length; i += 2)
                {
                    int randNum = Random.Range((int)Card.Number.Two, (int)Card.Number.Ace + 1);
                    int indexA = indexBuffer[i];
                    int indexB = indexBuffer[i + 1];
                    cardArray[indexA].number = (Card.Number)randNum;
                    cardArray[indexB].number = (Card.Number)randNum;
                }

                return cardArray;
            }
        }

        private void LateUpdate()
        {
            if (cardSelector2D.WasSelectionCompleted)
            {
                Card[] selectedCard = cardSelector2D.GetSelectedCards();
                CheckPairMatching(selectedCard[0], selectedCard[1]);
            }
        }

        private void CheckPairMatching(Card a, Card b)
        {
            if (a.number == b.number)
            {
                DeleteCard(a);
                DeleteCard(b);

                pairMatchingCount += 2;
                CheckEnd();
            }
            else
            {
                a.Flip();
                b.Flip();

                Debug.Log("두 카드가 다릅니다");
            }

            cardSelector2D.Clear();
        }

        private void DeleteCard(Card target)
        {
            for (int y = 0; y < board2D.GetLength(1); ++y)
            {
                for (int x = 0; x < board2D.GetLength(1); ++x)
                {
                    if (board2D[x, y] == null) continue;

                    if (board2D[x, y].Equals(target))
                    {
                        board2D[x, y] = null;
                        Destroy(target.gameObject);
                    }
                }
            }
        }

        private void CheckEnd()
        {
            if (pairMatchingCount >= board2D.Length)
            {
                clearObject.SetActive(true);
            }
        }
    }
}

