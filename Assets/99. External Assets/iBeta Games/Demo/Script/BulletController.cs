using UnityEngine;

namespace Geometry
{
    public class BulletController : MonoBehaviour
    {
        [field: SerializeField] public float Speed { get; private set; }
        private Vector3 foward = Vector3.right;
        public GameObject targetObject;

        private void Update()
        {
            if(targetObject == null)
            {
                Destroy(gameObject); 
                return;
            }
            transform.Translate(foward * Speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject == targetObject)
            {
                Destroy(gameObject);
            }
        }

        public void Set(Vector3 direction)
        {
            foward = direction;
        }

    }
}