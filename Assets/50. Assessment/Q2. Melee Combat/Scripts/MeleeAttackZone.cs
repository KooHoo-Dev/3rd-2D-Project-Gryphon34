using System.Collections.Generic;
using UnityEngine;

namespace Assessment.MeleeCombat
{
    // 사거리 안에 들어온 적을 들고 있다가, 스페이스를 누르면 전부 때린다.
    public class MeleeAttackZone : MonoBehaviour
    {
        [SerializeField] private int damage = 10;

        private readonly List<Enemy> targets = new List<Enemy>();

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Space)) return;

            for (int i = targets.Count - 1; i >= 0; i--)
            {
                Enemy target = targets[i];

                if (target.IsDead)
                {
                    targets.RemoveAt(i);
                    continue;
                }

                target.TakeDamage(damage);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy == null) return;

            targets.Add(enemy);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy == null) return;

            targets.Remove(enemy);
        }
    }
}
