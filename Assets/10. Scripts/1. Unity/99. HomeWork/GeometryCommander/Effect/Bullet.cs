using UnityEngine;

namespace Geometry
{
    public class Bullet : MonoBehaviour
    {
        public float speed = 1.0f;
        private GameObject targetObj;
        private Vector3 foward = Vector3.right;

        private void Update()
        {
            if(targetObj == null)
            {
                Destroy(gameObject);
                return;
            }
            foward=(targetObj.transform.position - transform.position).normalized;

            transform.right = foward;

            transform.position += foward * speed * Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject == targetObj.gameObject)
            {
                Destroy(gameObject);
            }
        }

        public void SetTarget(GameObject target, Vector3 direction)
        {
            targetObj = target;
            foward = direction;
        }
    }
}