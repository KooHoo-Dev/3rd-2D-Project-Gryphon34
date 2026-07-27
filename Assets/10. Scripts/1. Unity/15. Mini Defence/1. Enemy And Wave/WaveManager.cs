using System;
using System.Collections;
using Jay;
using UnityEngine;

namespace Study.MiniDefence
{
    public class WaveManager : SingletonBase<WaveManager>
    {
        public event Action<Transform, EnemyDef> EnemyReachedGoal;
        
        private WaveDef currentWave;
        private MapData currentMap;

        // WaveManager가 동작하기 위한 EntryPoint
        public void StartWave(WaveDef wave, MapData map)
        {
            currentWave = wave;
            currentMap = map;
            StartCoroutine(WaveCoroutine());
        }

        // Enemy 한 개체에 대한 스폰 처리 함수
        private void SpawnOne()
        {
            // Enemy 정보를 가지고 온다
            EnemyDef def = currentWave.Enemy;
            Enemy enemy = Instantiate(def.Prefab);
            enemy.Init(def, currentMap.GetWorldRoutePoints());
            enemy.GoalReached += HandleGoalReached;
        }
        
        private IEnumerator WaveCoroutine()
        {
            Debug.Log("Starting wave");
            // 웨이브 정보에 따른 딜레이 처리
            yield return new WaitForSeconds(currentWave.Delay);

            // 웨이브 정보에 정의된 숫자만큼
            for (int i = 0; i < currentWave.Count; i++)
            {
                // 소환하고
                SpawnOne();
                Debug.Log("Spawning " + i + " of " + currentWave.Count + " enemies");
                // 웨이 정보에 정의된 간격(시간)만큼 대기한다
                yield return new WaitForSeconds(currentWave.Interval);
            }
            
            Debug.Log("Wave finished");
        }
        
        // 아래의 함수는 enemy가 도달했는지 여부를 체크하는 함수고
        // 외부 클래스에서 Wave에서 빠져나간(Leaked) Enemy를
        // 알수있게 하나더 빼줍니다.
        private void HandleGoalReached(Enemy enemy)
        {
            Debug.Log($"{enemy.gameObject.name} Goal reached");
            // 구독하고 있던 함수(HandleGoalReached)를 제거해준다.
            enemy.GoalReached -= HandleGoalReached;
            
            // 최종적으로 구독자에게 목적지에 도달한 Enemy가 있음을
            // 중계 해준다. 받는 쪽은 해당정보를 가지고 게임적인 처리를 합니다
            // 피를 감소시킨다던가, 골드 페널티를 준다던가 등등
            EnemyReachedGoal?.Invoke(enemy.transform, currentWave.Enemy);
        }
    }
}