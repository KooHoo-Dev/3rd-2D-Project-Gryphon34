using System.Collections;
using UnityEngine;

namespace Geometry
{
    public class FallingBomb : MonoBehaviour
    {
        private float fallHeight = 10f; // 폭탄이 떨어지기 시작할 높이
        private float fallDuration = 0.9f; // 떨어지는데 걸리는 시간

        private void Start()
        {
            StartCoroutine(FallRoutine());
        }

        private IEnumerator FallRoutine()
        {
            Vector3 targetPos = transform.position;

            Vector3 startPos = targetPos + new Vector3(0, fallHeight, 0);

            transform.position = startPos;

            float timer = 0f;
            while (timer < fallDuration)
            {
                timer += Time.deltaTime;
                float t = timer / fallDuration;

                transform.position = Vector3.Lerp(startPos, targetPos, t);

                yield return null;
            }

            Destroy(gameObject);
        }
    }
}