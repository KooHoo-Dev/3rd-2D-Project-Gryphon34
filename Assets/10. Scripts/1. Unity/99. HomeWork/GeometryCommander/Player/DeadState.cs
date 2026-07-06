using System.Collections;
using UnityEngine;

namespace Geometry
{
    public class DeadState : MonoBehaviour
    {
        private float moraleRange = 2f;
        [SerializeField]private LayerMask unitLayer;
        public void OnEnable()
        {
            StartCoroutine(DeadAnim());
        }

        public IEnumerator DeadAnim()
        {
            yield return new WaitForSeconds(0.3f);
            PlayerUnit unit = GetComponentInParent<PlayerUnit>();
            Collider2D[] hitUnits = Physics2D.OverlapCircleAll(transform.position, moraleRange, unitLayer);

            if(hitUnits.Length > 0)
            {
                foreach(Collider2D collider in hitUnits)
                {
                    PlayerUnit targetUnit = collider.GetComponent<PlayerUnit>();
                    if(targetUnit !=null &&targetUnit !=unit&&targetUnit.current_Hp>0)
                    targetUnit.current_Morale -= 5.0f;
                }
            }
            if(unit != null)
            {
                PlayerManager.Instance.Dead(unit);
            }
        }
    }
}