using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEditor.AnimatedValues;
using UnityEditor.Experimental.GraphView;

namespace Study.MergeGame
{
    public class MergeGameManager : MonoBehaviour
    {
        // static 키워드는 랜드마크와 같습니다.
        // 프로그램에서 정적(고정되어있다~)으로 선언되어있어서
        // 전역적인 접근을 허용합니다.
        public static MergeGameManager Instance;

        [Header("GameOver Settings")]
        public GameObject[] enableObjects;

        public GameObject[] disableObjects;

        public AnimalBall[] BallPrefabs;
        public int maxIndexInQueue = 4;

        private Queue<int> ballIndexQueue = new Queue<int>();


        #region Unity Methods
        private void Awake()
        {
            // 약식 싱글톤 선언
            // 특정 한 객체만 남기고 다 지워버린다~
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            //범위 에러 처리
            maxIndexInQueue = Mathf.Min(maxIndexInQueue, BallPrefabs.Length);
            maxIndexInQueue = Mathf.Max(maxIndexInQueue, 0);
            ballIndexQueue.Enqueue(Random.Range(0, maxIndexInQueue));
        }

        #endregion

        // 큐(ballIndexQueue)에서 꺼내온 녀석을 반환 합니다. MergeGameController에서 호출합니다
        public AnimalBall GetNextBall()
        {
            // 인덱스를 하나 뽑아주고
            int dequeueIndex = ballIndexQueue.Dequeue();
            // BallPrefabs의 길이만큼 랜덤한 숫자를 하나 뽑아서 큐에 담아줍니다
            // 한번에 여러개 담는 처리해도 상관없음
            ballIndexQueue.Enqueue(Random.Range(0, maxIndexInQueue));

            AnimalBall ball = GetBall(dequeueIndex);
            return ball;
        }

        private AnimalBall GetBall(int index)
        {
            // 예외처리
            if (index < 0 || BallPrefabs.Length <= index) return BallPrefabs[0];
            return BallPrefabs[index];
        }

        public void MergeBall(AnimalBall ballA, AnimalBall ballB)
        {
            // 두 개체가 병합 진행상태임을 지정해줌
            ballA.IsMerged = true;
            ballB.IsMerged = true;

            // 삭제하기 전에 중간 지점을 지역변수로 저장합니다
            Vector3 centerPosition = ballA.transform.position + ballB.transform.position;
            centerPosition /= 2;

            // 생성할 준비하기
            int upgradeLevel = ballA.level;
            AnimalBall upgradeBall = GetBall(upgradeLevel);

            // ballA와 ballB를 삭제
            ballA.gameObject.SetActive(false);
            ballB.gameObject.SetActive(false);
            // 두개체를 비활성화 해주는 이유는 삭제가 조금 느려서
            // 충돌 안일어나게 하려고 일단 비활성화 시켜두고, 안전하게 삭제 처리함

            Destroy(ballA.gameObject);
            Destroy(ballB.gameObject);

            AnimalBall spawnedBall = Instantiate(upgradeBall, centerPosition, Quaternion.identity);
            spawnedBall.Drop();

            AddScore(upgradeLevel);
        }

        // 스코어를 더해줄때는 새로 생성될 개체의 level을 기준으로 득점처리를 진행합니다
        // 

        public TMP_Text text;
        private Dictionary<int,int> scoreTable= new Dictionary<int,int>();
        private int score = 0;

        private void Start()
        {
            scoreTable.Add(1, 10);
            scoreTable.Add(2, 20);
            scoreTable.Add(3, 40);
            scoreTable.Add(4, 50);
            scoreTable.Add(5, 55);
            scoreTable.Add(6, 70);
            scoreTable.Add(7, 77);
            scoreTable.Add(8, 80);
            scoreTable.Add(9, 100);
            text.SetText($"{score}");
        }

        private void AddScore(int upgradeLevel)
        {
            score += scoreTable[upgradeLevel];
            text.SetText($"{score}");
        }
        public void GameOver()
        {
            for(int i = 0; i < enableObjects.Length; ++i)
            {
                enableObjects[i].SetActive(true);
            }
            for(int i=0;i< disableObjects.Length; ++i)
            {
                disableObjects[i].SetActive(false);
            }
        }
    }
}

