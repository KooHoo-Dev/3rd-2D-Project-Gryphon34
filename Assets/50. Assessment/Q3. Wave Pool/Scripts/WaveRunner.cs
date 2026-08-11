using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 1. waveIndex가 static 함수여서 씬을 로드해도 이전 데이터가 남아있어 이전 웨이브의 값을 불러오게 된다. R을 눌렀을 때 waveIndex를 초기화 해준다.
// 2. Enemy가 죽었을 때 비활성화 되며 죽었을 때의 currentHealth 값을 가지고 비활성화가 된다. Wave 시작시 EnemyPool에 있는 Enemy가 활성화 되며 이전에 죽은 Enemy들이 활성화가 되며
//    cuttentHealth가 0인 Enemy가 활성화 되어 곧바로 죽어버리게 된다. 이를 위해 Disable이 되면 currentHealth를 다시 maxHealth로 초기화 하는 작업을 해준다.

namespace Assessment.WavePool
{
    // 살아 있는 적이 전부 사라지면 다음 웨이브로 넘어간다.
    public class WaveRunner : MonoBehaviour
    {
        [SerializeField] private EnemyPool pool;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private int baseEnemyCount = 3;

        private static int waveIndex;

        private readonly List<PooledEnemy> alive = new List<PooledEnemy>();

        public static int CurrentWave => waveIndex;

        private void Start()
        {
            StartNextWave();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                waveIndex = 0;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                return;
            }

            for (int i = alive.Count - 1; i >= 0; i--)
            {
                if (!alive[i].IsDead) continue;

                Debug.Log($"{alive[i].name} 처치 — 남은 적 {alive.Count - 1}마리");

                pool.Release(alive[i]);
                alive.RemoveAt(i);
            }

            if (alive.Count == 0) StartNextWave();
        }

        private void StartNextWave()
        {
            waveIndex++;

            int count = baseEnemyCount + (waveIndex - 1);

            Debug.Log($"웨이브 {waveIndex} 시작 — 적 {count}마리 소환");

            for (int i = 0; i < count; i++)
            {
                Vector3 position = spawnPoints[i % spawnPoints.Length].position;

                alive.Add(pool.Get(position));
            }
        }
    }
}
