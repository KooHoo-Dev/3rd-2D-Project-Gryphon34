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
        public GameObject clearUI;
        public GameObject failUI;

        protected override void Awake()
        {
            base.Awake();
            clearUI.SetActive(false);
            failUI.SetActive(false);
        }

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
            

            StopSystem();
            clearUI.SetActive(true);
        }

        public void GameOver()
        {
            if (IsgameClear || IsgameOver) return;
            IsgameOver = true;

            StopSystem();
            failUI.SetActive(true);
        }

        public void StopSystem()
        {
            EnemyUnit[] enemies = FindObjectsByType<EnemyUnit>(FindObjectsSortMode.None);
            foreach (EnemyUnit enemy in enemies) Destroy(enemy.gameObject);

            if (playerBase != null) Destroy(playerBase.gameObject);

            PlayerUnit[] units = FindObjectsByType<PlayerUnit>(FindObjectsSortMode.None);
            foreach (PlayerUnit unit in units) Destroy(unit.gameObject);

            if (commandSystem != null) commandSystem.enabled = false;

            if (spawnManager != null) spawnManager.enabled = false;

            if(playerManager != null) playerManager.enabled = false;
        }
    }
}