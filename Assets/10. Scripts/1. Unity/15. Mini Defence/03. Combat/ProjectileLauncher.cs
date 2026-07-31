using System;
using UnityEngine;

namespace Study.MiniDefence
{
    // 이 유닛이 "무엇을, 어디서" 쏘는지 아는 컴포넌트. 유닛 프리팹에 붙인다.
    // 투사체를 쏘지 않는 유닛은 이 컴포넌트를 붙이지 않는다. 즉시 명중으로 처리된다.
    public class ProjectileLauncher : MonoBehaviour
    {
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private Transform firePoint;

        public void Launch(Enemy target, Action<Vector3> onArrive)
        {
            ProjectileSystem.Launch(projectilePrefab, firePoint.position, target, onArrive);
        }
    }
}