using System;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Study.MiniDefence
{
    public class Enemy : MonoBehaviour
    {
        private EnemyDef refDef;
        private Queue<Vector3> moveQueue;
        private Vector3 currentGoal;
        private Animator animator;

        // 목표지점에 도착했을때 알림을 통지할 델리게이트.
        // 외부 클래스에서는 해당 델리게이트를 구독해서 알맞은 처리를 진행한다
        public event Action<Enemy> GoalReached;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            MoveNext();
        }

        private void OnDisable()
        {
            // Enemy가 만약에 체력이 닳아서 죽게된다면 진행중인 Tween을 없애준다
            // (자동으로 되는지 모르겠음. 아마 안될껄?)
            transform.DOKill();
        }

        public void Init(EnemyDef def, List<Vector3> waypoints)
        {
            // 매개변수로 전달된 EnemyDef 객체(레퍼런스 타입)를 캐싱한다
            refDef = def;

            moveQueue = new Queue<Vector3>();
            for (int i = 0; i < waypoints.Count; i++)
                moveQueue.Enqueue(waypoints[i]);
            // moveQueue에서 가장 첫번째 값(waypoints[0]) 위치로 이동한다 
            transform.position = moveQueue.Dequeue();
        }

        private void MoveNext()
        {
            if (moveQueue.Count == 0)
            {
                // 도착 처리
                GoalReached?.Invoke(this); //내가(this) 목표지점에 도착했음을 알려준다
                gameObject.SetActive(false);
                return;
            }
            // 이동할 웨이포인트가 남았다면
            // moveQueue에서 하나를 뽑아서 목적지를 갱신하고
            currentGoal = moveQueue.Dequeue();
            // 이동 처리를 한다
            transform.DOMove(currentGoal, 1/refDef.Speed)
                .SetEase(Ease.Linear)
                .OnComplete(MoveNext);
        }
    }
}