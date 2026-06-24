using UnityEngine;

namespace Study.PrimitiveAndVector
{
    public class Study_Physics : MonoBehaviour
    {

        // # Physics?
        // - 가상 공간에서 GameObject들이 현실적인 물리 법칙에 따라 상호작용하도록
        //   시뮬레이션 하는 시스템입니다.
        // - 중력, 충돌, 마찰, 반발력 등 실제 세계의 물리 현상을 게임에 적용하여
        //   더욱 생동감 있고 반응적인 게임 플레이를 가능하게 합니다.
        // - Physics 클래스 안의 멤버(함수)를 이용해서 프로젝트마다 적합한
        //   물리법칙을 수동적으로 만들 수 있습니다.
        // - Rigidbody와 Collider등을 검색하거나 제어하는데에 활용합니다.

        private void Start()
        {
            StudyPhysics();
        }


        private void StudyPhysics()
        {
            // Physics2D의 주요 함수들
            // - Cast : 특정 모형을 발사하는것
            // - Overlap : 특정 모형을 중심으로 영역내를 감지하는것 (덮어씌어 보는 것)

            // 모형?
            // - Ray(광선), Circle(원), Box(사각형)
            Vector3 startPoint = rayTransform.position;
            Vector3 direction = Vector3.right;
            RaycastHit2D hit = Physics2D.Raycast(startPoint, direction);

            if (hit.collider != null)
            {
                Debug.Log($"{hit.collider.name}에 적중");
            }

        }

        public Transform rayTransform;
        public float rayDistance = 100;

        public Transform boxTransform;
        public Vector3 boxSize;

        public float radius = 1;

        // OnDrawGizmos()
        // : 해당 이벤트 함수는 우리가 보는 SceneView에 특정 모형이나 도형
        // 을 그리는 데에 사용되는 함수입니다. Play로 실행할 경우 출력되지 않음
        private void OnDrawGizmos()
        {
            /*Vector3 startPoint = rayTransform.position;
            Vector3 direction = Vector3.right;
            RaycastHit2D hit = Physics2D.Raycast(startPoint, direction);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(startPoint, startPoint + (direction* rayDistance));

            if (hit.collider != null)
            {
                Debug.Log($"{hit.collider.name}에 적중");
            }

            Vector3 boxPoint = boxTransform.position;
            RaycastHit2D hitBox = Physics2D.BoxCast(boxPoint, boxSize / 2, boxTransform.rotation.eulerAngles.z,Vector3.right);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(boxPoint, boxSize);

            if (hitBox.collider != null)
            {
                Debug.Log($"BoxCast {hitBox.collider.name}에 적중");
            }
            */

            /*Collider2D resultoverlapBox = Physics2D.OverlapBox(boxTransform.position, boxSize, 0.0f);

            Collider2D[] resultOverlapBoxAll = Physics2D.OverlapBoxAll(boxTransform.position, boxSize, 0.0f);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(boxTransform.position, boxSize);

            if (resultoverlapBox != null)
            {
                Debug.Log($"OverlapBox : {resultoverlapBox.gameObject.name}");
            }

            foreach (Collider2D col in resultOverlapBoxAll)
            {
                Debug.Log($"OverlapBox : {col.gameObject.name}");
            }
            */

            Collider2D resultoverlapCIrcle = Physics2D.OverlapCircle(boxTransform.position, radius);

            Collider2D[] resultOverlapCIrcleAll = Physics2D.OverlapCircleAll(boxTransform.position, radius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(boxTransform.position, radius);

            if (resultoverlapCIrcle != null)
            {
                Debug.Log($"OverlapCircle : {resultoverlapCIrcle.gameObject.name}");
            }

            foreach (Collider2D col in resultOverlapCIrcleAll)
            {
                Debug.Log($"OverlapCircleAll : {col.gameObject.name}");
            }
        }
    }
}