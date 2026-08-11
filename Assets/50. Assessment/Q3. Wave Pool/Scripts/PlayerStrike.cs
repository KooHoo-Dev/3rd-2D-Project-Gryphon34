using UnityEngine;

namespace Assessment.WavePool
{
    // 마우스로 클릭한 적을 한 번 때린다.
    public class PlayerStrike : MonoBehaviour
    {
        [SerializeField] private Camera worldCamera;

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;

            Vector2 point = worldCamera.ScreenToWorldPoint(Input.mousePosition);

            Collider2D hit = Physics2D.OverlapPoint(point);
            if (hit == null) return;

            PooledEnemy enemy = hit.GetComponent<PooledEnemy>();
            if (enemy == null) return;

            enemy.TakeHit();
        }
    }
}
