using System.Collections.Generic;
using Study.Utilities;   // GameObjectPool
using UnityEngine;

namespace Study.MiniDefence
{
    // 투사체 발사 창구. (EnemyRegistry / UnitRegistry 처럼 static)
    // 반납은 여기서 만든 델리게이트가 알아서 처리하므로, 쏜 쪽도 투사체도 풀을 몰라도 된다.
    public static class ProjectileSystem
    {
        private static readonly Dictionary<Projectile, GameObjectPool<Projectile>> pools = new();

        public static void Launch(Projectile prefab, Vector3 from, Enemy target,
            System.Action<Vector3> onArrive)
        {
            GameObjectPool<Projectile> pool = GetOrCreatePool(prefab);

            Projectile projectile = pool.Get();

            // 투사체가 다 날아가면 스스로 pool.Release 를 부르도록 "돌아가는 방법"을 쥐어 준다.
            projectile.Launch(from, target, onArrive, () => pool.Release(projectile));
        }

        // 판을 다시 시작할 때 풀을 버린다.
        public static void Clear()
        {
            pools.Clear();
        }

        private static GameObjectPool<Projectile> GetOrCreatePool(Projectile prefab)
        {
            if (pools.TryGetValue(prefab, out GameObjectPool<Projectile> pool)) return pool;

            pool = new GameObjectPool<Projectile>(prefab);
            pools[prefab] = pool;
            return pool;
        }
    }
}