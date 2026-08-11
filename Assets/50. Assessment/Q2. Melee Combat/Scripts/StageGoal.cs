using UnityEngine;

// 1. EnemyB의 CircleCollider2D가 꺼져있어 충돌 판정이 되지 않았다. Enemy의 Awake()에 초기화 진행.
// 2. 유니티 인스펙터 상에서 requiredKills가 4로 되어있어 스테이지 클리어 조건이 맞지 않음.
namespace Assessment.MeleeCombat
{
    public class StageGoal : MonoBehaviour
    {
        [SerializeField] private int requiredKills = 3;

        private int killCount;

        public void ReportKill(Enemy enemy)
        {
            killCount++;

            Debug.Log($"{enemy.name} 처치 — 남은 적 {requiredKills - killCount}마리");

            if (killCount < requiredKills) return;

            Debug.Log("스테이지 클리어");
        }
    }
}
