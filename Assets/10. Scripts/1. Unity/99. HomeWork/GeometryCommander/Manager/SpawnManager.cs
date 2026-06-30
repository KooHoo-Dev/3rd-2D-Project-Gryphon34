using System.Collections;
using UnityEngine;

namespace Geometry
{
    public class SpawnManager : MonoBehaviour
    {
        // 위치 하나를 기준으로 y 값 +3.5, -3.5
        // 랜덤 값으로 스폰 위치 정함
        // 코루틴으로 3초? 마다 소환

        public int spawnCount = 3;

        public Transform deafultPos;
        private Vector3 spawnPos;

        [SerializeField]
        private GameObject[] spawnList;

        private void Start()
        {
            StartCoroutine(SpawnTimer());
        }

        private void SetSpawnPos()
        {
            // 일단 기준 위치를 지정
            // 기준 위치에 +3.5, -3.5를 한 값을 생성
            // 그것을 VEctor3로 지정
            // 

            float randNum = Random.Range(-3.5f, 3.5f);
            spawnPos = deafultPos.transform.position + new Vector3(0, randNum, 0);
        }

        private IEnumerator SpawnTimer()
        {
            while (true)
            {
                for (int i = 0; i < spawnCount; ++i)
                {
                    SetSpawnPos();
                    int randNum = Random.Range(0, 3);
                    GameObject.Instantiate(spawnList[randNum], spawnPos, Quaternion.identity);
                }
                yield return new WaitForSeconds(5f);
            }
        }
    }
}