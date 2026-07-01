using Study_ActionPlatformer;
using UnityEngine;
namespace iBetaGames
{
    public class BulletController : MonoBehaviour
    {
        [field: SerializeField] public float Speed { get; private set; }
        private Vector3 foward = Vector3.right;

        private void Update()
        {
            transform.Translate(foward * Speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject == Player.LocalPlayer.gameObject)
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