using Study.Utilities;
using UnityEngine;

namespace Study_2D_DirectionAndRotation
{
    public class Study_Transform2D : MonoBehaviour
    {
        [field: SerializeField] private Transform Circle { get; set; }
        private void Update()
        {
            // 삼각형을 회전 시키는 방법

            // 바라보게 할 위치
            Vector3 goalPosition = Circle.position;
            // 내 위치
            Vector3 myPosition = transform.position;

            // 방향벡터 구하기
            Vector3 dir = (goalPosition - myPosition).normalized;
            transform.up = dir;
        }
    }
}