using Jay;
using UnityEngine;

namespace Geometry
{
    public class GameManager : SingletonBase<GameManager>
    {
        [SerializeField] private Base playerBase;
        [SerializeField] public float goalTime = 180f;
        [SerializeField] private CommandSystem commandSystem;
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private PlayerManager playerManager;
        public bool IsgameOver = false;
        public bool IsgameClear = false;

        private void Update()
        {
            if (IsgameOver || IsgameClear) return;

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
            if (IsgameClear || IsgameOver) return;
            IsgameClear=true;
            EnemyUnit[] enemies =FindObjectsByType<EnemyUnit>(FindObjectsSortMode.None);
            foreach(EnemyUnit enemy in enemies)
            {
                Destroy(enemy.gameObject);
            }

            StopSystem();
        }

        public void GameOver()
        {
            if (IsgameClear || IsgameOver) return;
            IsgameOver = true;

            if(playerBase != null) Destroy(playerBase.gameObject);

            PlayerUnit[] units = FindObjectsByType<PlayerUnit>(FindObjectsSortMode.None);
            foreach(PlayerUnit unit in units) Destroy(unit.gameObject);

            StopSystem();

        }

        public void StopSystem()
        {
            if (commandSystem != null) commandSystem.enabled = false;

            if (spawnManager != null) spawnManager.enabled = false;

            if(playerManager != null) playerManager.enabled = false;
        }
    }
}