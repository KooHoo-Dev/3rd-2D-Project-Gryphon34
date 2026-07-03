using System.Collections;
using UnityEngine;

namespace Geometry
{
    public class EnemyDeadState : MonoBehaviour
    {
        public void OnEnable()
        {
            StartCoroutine(DeadAnim());
        }

        public IEnumerator DeadAnim()
        {
            yield return new WaitForSeconds(0.5f);
            EnemyUnit unit = GetComponentInParent<EnemyUnit>();
            if (unit != null)
            {
                PlayerManager.Instance.Dead(unit);
            }
        }
    }
}