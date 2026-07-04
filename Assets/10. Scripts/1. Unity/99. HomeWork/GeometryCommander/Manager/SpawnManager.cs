using System.Collections;
using TMPro;
using UnityEngine;

namespace Geometry
{
    public class SpawnManager : MonoBehaviour
    {
        // 위치 하나를 기준으로 y 값 +3.5, -3.5
        // 랜덤 값으로 스폰 위치 정함
        // 코루틴으로 3초? 마다 소환

        public int spawnCount = 3;

        public Transform defaultPos;
        private Vector3 spawnPos;
        public float gap = 2f;
        [SerializeField]
        private GameObject[] spawnList;

        private void Start()
        {
            StartCoroutine(SpawnTimer());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }


        private IEnumerator SpawnTimer()
        {
            while (true)
            {
                yield return new WaitForSeconds(2f);
                float startY = defaultPos.position.y;

                for (int i = 0; i < spawnCount; ++i)
                {
                    int randNum = Random.Range(0, 3);
                    Vector3 spawnPos = new Vector3(defaultPos.position.x, startY + (gap * i), defaultPos.position.z);
                    GameObject.Instantiate(spawnList[randNum], spawnPos, Quaternion.identity);
                }
                yield return new WaitForSeconds(10f);
            }
        }
    }
}