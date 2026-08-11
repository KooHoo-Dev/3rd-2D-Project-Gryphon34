using UnityEngine;

namespace Assessment.MeleeCombat
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 30;

        private int currentHealth;
        private StageGoal goal;
        private CircleCollider2D circle;

        public bool IsDead => currentHealth <= 0;

        private void Awake()
        {
            currentHealth = maxHealth;
            circle = GetComponent<CircleCollider2D>();
            circle.enabled = true;
            goal = FindAnyObjectByType<StageGoal>();
        }

        public void TakeDamage(int amount)
        {
            currentHealth -= amount;

            Debug.Log($"{name} 피격 — 남은 체력 {currentHealth}");

            if (currentHealth > 0) return;

            goal.ReportKill(this);
            gameObject.SetActive(false);
        }
    }
}
