using UnityEngine;
using UnityEngine.InputSystem;
using Study.CardSelector;

namespace Study.PairMatchingGame
{
    public class CardSelector2D : MonoBehaviour
    {
        public enum Direction
        {
            Up, Down, Left, Right
        }

        [Header("Ref Object")]
        public UnityEngine.Transform cursor;

        [Header("Settings")]
        public float cursorYOffset = -0.5f;

        private Card[,] cards;

        private Card selectCardA;
        private Card selectCardB;

        public bool WasSelectionCompleted { get; private set; }

        private void Update()
        {
            WasSelectionCompleted = false;

            if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                MoveCursor(Direction.Left);
            }
            else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                MoveCursor(Direction.Right);
            }
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                MoveCursor(Direction.Up);
            }
            else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                MoveCursor(Direction.Down);
            }


            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                SelectCard();
            }
        }

        #region Public Methods

        public void SetBoard(Card[,] cardArray)
        {
            cards = cardArray;
        }

        public Card[] GetSelectedCards()
        {
            return new[] { selectCardA, selectCardB };
        }

        public void Clear()
        {
            selectCardA = null;
            selectCardB = null;
        }

        #endregion

        #region Private Methods

        private int currentX = 2;
        private int currentY = 0;

        private void SelectCard()
        {
            Card currentCard = cards[currentX, currentY];

            if (selectCardA == null)
            {
                selectCardA = currentCard;
            }
            else if (selectCardA == currentCard)
            {
                return;
            }
            else
            {
                selectCardB = currentCard;
                WasSelectionCompleted = true;
            }

            cards[currentX, currentY].Flip();
        }

        private void MoveCursor(Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    currentY++;
                    break;
                case Direction.Down:
                    currentY--;
                    break;
                case Direction.Left:
                    currentX--;
                    break;
                case Direction.Right:
                    currentX++;
                    break;
            }

            //예외처리 구간
            if (currentY >= cards.GetLength(1)) currentY = 0;    //행의 최대값을 벗어나면 0번위치로
            else if (currentY < 0) currentY = cards.GetLength(1) - 1; //행의 최소값을 벗어나면 끝위치로

            if (currentX >= cards.GetLength(0)) currentX = 0;    //행의 최대값을 벗어나면 0번위치로
            else if (currentX < 0) currentX = cards.GetLength(0) - 1; //행의 최소값을 벗어나면 끝위치로

            UpdateCursorPosition(currentX, currentY);
        }

        private void UpdateCursorPosition(int x, int y)
        {
            float cardX = cards[x, y].transform.position.x;
            float cardY = cards[x, y].transform.position.y + cursorYOffset;
            cursor.position = new Vector3(cardX, cardY, cursor.position.z);
        }



        #endregion
    }
}

