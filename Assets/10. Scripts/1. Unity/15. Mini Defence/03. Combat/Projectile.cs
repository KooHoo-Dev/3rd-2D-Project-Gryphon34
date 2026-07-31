using System;
using UnityEngine;

namespace Study.MiniDefence
{
    // 유닛이 쏘는 투사체. 대상을 향해 날아가, 도착하면 "무슨 일이 벌어질지"를 실행한다.
    //
    // 투사체는 피해 계산을 모른다. 그저 "여기까지 날아가서, 도착하면 이걸 해라"를 전달받을 뿐이다.
    [RequireComponent(typeof(SpriteRenderer))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 12f;              
        [SerializeField] private float arriveDistance = 0.15f;   // 이 거리 안에 들면 "도착"으로 본다
        [SerializeField] private GameObject impactEffect;        // 착탄 순간 터뜨릴 이펙트. 없으면 안 만든다

        private Enemy target;             
        private Vector3 aimPoint;         

        private Action<Vector3> onArrive; 
        private Action release;           

        // 발사. 대상을 향해 날아가기 시작한다.
        public void Launch(Vector3 from, Enemy target, Action<Vector3> onArrive, Action release)
        {
            transform.position = from;
            this.target = target;
            this.aimPoint = target.transform.position;
            this.onArrive = onArrive;
            this.release = release;
        }

        private void Update()
        {
            // 대상이 살아 있으면 그 위치를 계속 갱신해 쫓는다
            // 대상이 사라졌으면 마지막으로 겨눈 지점을 향해 그대로 날아간다.
            if (target != null && target.gameObject.activeInHierarchy && target.IsDead == false)
                aimPoint = target.transform.position;

            Vector3 toAim = aimPoint - transform.position;

            // 진행 방향을 바라보게 회전시킨다.
            if (toAim.sqrMagnitude > 0.0001f)
            {
                float angle = Mathf.Atan2(toAim.y, toAim.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }

            transform.position = Vector3.MoveTowards(transform.position, aimPoint, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, aimPoint) <= arriveDistance)
                Arrive();
        }

        private void Arrive()
        {
            // 착탄 이펙트를 그 자리에 남긴다.
            if (impactEffect != null)
                Instantiate(impactEffect, transform.position, Quaternion.identity);

            onArrive(transform.position);   // 무슨 일이 벌어질지는 쏜 쪽이 정한 대로
            release();                      
        }
    }
}