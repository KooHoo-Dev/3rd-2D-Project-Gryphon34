using System;
using System.Collections;
using Jay;
using Study.MiniDefence._3._Combat;
using UnityEngine;

namespace Study.MiniDefence
{
    public class WaveManager : SingletonBase<WaveManager>
    {
        private WaveDef currentWave;
        private MapData currentMap;

        public event Action<Transform, EnemyDef> EnemyReachedGoal;
        public event Action<WaveDef> WaveCleared;
        public event Action<int> WaveChanged;

        public int CurrentWaveNo { get; private set; } = 0;
        public bool IsWaveActive { get; private set; }
        
        // 스폰 자체는 끝났는지 체크하는 플래그
        private bool spawnFinished;

        private void OnEnable()
        {
            EnemyRegistry.UnRegistered += HandleCheckWaveEnd;
        }

        private void OnDisable()
        {
            EnemyRegistry.UnRegistered -= HandleCheckWaveEnd;
        }

        // EnemyRegistry.UnRegistered 이벤트의 구독, 구독해제를 위해
        // CheckWaveEnd를 한번 감싸준 함수
        private void HandleCheckWaveEnd(Enemy enemy)
        {
            CheckWaveEnd();
        }
        
        // 뿌리기(적 스폰)가 끝났고 남은적도 없어야 웨이브가 끝난것.
        private void CheckWaveEnd()
        {
            if (IsWaveActive == false) return; // 웨이브 진행중이 아니면 Pass
            if (spawnFinished == false) return; // 스폰이 진행중이면 Pass
            if (EnemyRegistry.AliveCount > 0) return; // 남은 적이 있다면 Pass

            IsWaveActive = true;
            // 원래는 보상테이블이 따로 있는데, 컬럼이 하나여서 그냥 값으로 대체하겠습니다
            // ID가 아니라 그냥 값입니다. 이렇게 하시면 안됩니다
            EconomyManager.instance.Earn(currentWave.RewardID);
            WaveCleared?.Invoke(currentWave);
            
            Debug.Log($"[WaveManager] 웨이브 : {currentWave.ID} 클리어!!, " +
                      $"{currentWave.RewardID} 획득!");
        }

        // WaveManager가 동작하기 위한 EntryPoint
        public void StartWave(WaveDef wave, MapData map)
        {
            currentWave = wave;
            currentMap = map;

            IsWaveActive = true;
            spawnFinished = false;
            
            WaveChanged?.Invoke(currentWave.ID);
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
                //Debug.Log("Spawning " + i + " of " + currentWave.Count + " enemies");
                // 웨이 정보에 정의된 간격(시간)만큼 대기한다
                yield return new WaitForSeconds(currentWave.Interval);
            }

            spawnFinished = true;
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

        public void Restore(int waveNo)
        {
            CurrentWaveNo = waveNo;
            WaveChanged?.Invoke(CurrentWaveNo);
        }
    }
}