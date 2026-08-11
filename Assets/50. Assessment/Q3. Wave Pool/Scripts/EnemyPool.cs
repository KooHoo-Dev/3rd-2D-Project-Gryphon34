using System.Collections.Generic;
using UnityEngine;

namespace Assessment.WavePool
{
    // 적을 매번 만들고 지우는 대신 꺼내 쓰고 되돌려 받는다.
    public class EnemyPool : MonoBehaviour
    {
        [SerializeField] private PooledEnemy enemyPrefab;
        [SerializeField] private int prewarmCount = 4;

        private readonly Queue<PooledEnemy> idle = new Queue<PooledEnemy>();

        private void Awake()
        {
            for (int i = 0; i < prewarmCount; i++)
            {
                PooledEnemy created = Instantiate(enemyPrefab, transform);
                created.name = $"Enemy_{i}";
                created.gameObject.SetActive(false);
                idle.Enqueue(created);
            }
        }

        public PooledEnemy Get(Vector3 position)
        {
            PooledEnemy enemy;

            if (idle.Count > 0)
            {
                enemy = idle.Dequeue();
            }
            else
            {
                enemy = Instantiate(enemyPrefab, transform);
                enemy.name = $"Enemy_{transform.childCount - 1}";
            }

            enemy.transform.position = position;
            enemy.gameObject.SetActive(true);

            return enemy;
        }

        public void Release(PooledEnemy enemy)
        {
            enemy.gameObject.SetActive(false);
            idle.Enqueue(enemy);
        }
    }
}
