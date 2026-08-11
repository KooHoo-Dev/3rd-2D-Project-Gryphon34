using UnityEngine;

namespace Assessment.WavePool
{
    public class PooledEnemy : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 3;

        private int currentHealth;

        public bool IsDead => currentHealth <= 0;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        private void OnDisable()
        {
            currentHealth = maxHealth;
        }

        public void TakeHit()
        {
            currentHealth--;

            Debug.Log($"{name} 피격 — 남은 체력 {currentHealth}");
        }
    }
}
