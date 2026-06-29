using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] protected float maxHp=100f;
    [SerializeField] protected float currentHp=100f;

    public void BaseTakeDamage(float damage)
    {
        currentHp -= damage;
        if(currentHp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
