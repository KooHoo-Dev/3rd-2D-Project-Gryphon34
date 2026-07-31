using UnityEngine;

namespace Study.MiniDefence
{
    // 유닛의 공격 사거리를 원으로 그려서 보여준다.
    //
    // 원을 그리는 원리
    // : 한 바퀴(360도 = 2π)를 여러 조각으로 잘게 나누고,
    //  각 각도마다 (cos, sin) × 반지름 위치에 점을 찍어 이어 붙이면 원처럼 보인다.
    //  조각이 많을수록 매끄럽고, 적을수록 각져 보인다.
    [RequireComponent(typeof(LineRenderer))]
    public class RangeIndicator : MonoBehaviour
    {
        [SerializeField] private int segments = 48;   // 원을 몇 조각으로 나눌지

        private LineRenderer line;

        private void Awake()
        {
            line = GetComponent<LineRenderer>();

            line.loop = true;             // 마지막 점과 첫 점을 이어 닫힌 원으로
            line.useWorldSpace = true;

            Hide();
        }

        // 지정한 위치에 반지름 radius 짜리 원을 그린다.
        public void Show(Vector3 center, float radius)
        {
            line.positionCount = segments;

            for (int i = 0; i < segments; i++)
            {
                float angle = (2f * Mathf.PI * i) / segments;

                float x = center.x + (Mathf.Cos(angle) * radius);
                float y = center.y + (Mathf.Sin(angle) * radius);

                line.SetPosition(i, new Vector3(x, y, center.z));
            }

            line.enabled = true;
        }

        public void Hide()
        {
            line.enabled = false;
        }
    }
}