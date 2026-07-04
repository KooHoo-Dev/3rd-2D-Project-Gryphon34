using System.Collections;
using UnityEngine;

namespace Geometry
{
    public class DeadState : MonoBehaviour
    {
        public void OnEnable()
        {
            StartCoroutine(DeadAnim());
        }

        public IEnumerator DeadAnim()
        {
            yield return new WaitForSeconds(0.3f);
            PlayerUnit unit = GetComponentInParent<PlayerUnit>();
            if(unit != null)
            {
                PlayerManager.Instance.Dead(unit);
            }
        }
    }
}