using Jay;
using UnityEngine;

namespace Geometry
{
    public class GameManager : SingletonBase<GameManager>
    {
        [SerializeField] private Base playerBase;
        [SerializeField] private float goalTime = 180f;
        private bool IsgameOver = false;

        private void Update()
        {
            if (IsgameOver)
            {
                Debug.LogWarning("게임 오버");
            }
            if (goalTime > 0)
            {
                goalTime-=Time.deltaTime;
            }

            if (goalTime < 0)
            {
                goalTime = 0;
                GameClear();
            }
        }

        private void GameClear()
        {
            Debug.Log("게임 클리어");
        }

        public void GameOver()
        {
            IsgameOver = true;
            Destroy(playerBase.gameObject);
        }

    }
}