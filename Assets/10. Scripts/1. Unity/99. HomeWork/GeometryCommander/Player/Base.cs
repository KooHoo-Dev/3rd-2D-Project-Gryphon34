using Geometry;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] protected float maxHp=100f;
    [SerializeField] public float currentHp=100f;
    public Transform hpBar;
    private float hpX;
    private float hpY;

    private void Awake()
    {
        hpX = hpBar.transform.localScale.x;
        hpY = hpBar.transform.localScale.y;
    }

    private void Update()
    {
        updateHpBar();
    }

    public void BaseTakeDamage(float damage)
    {
        currentHp -= damage;
        if(currentHp <= 0)
        {
            GameManager.Instance.GameOver();
        }
    }
    public void updateHpBar()
    {
        if (hpBar != null)
        {
            float hpRatio = currentHp / maxHp;
            hpBar.transform.localScale = new Vector3(hpRatio * hpX, hpY, 1f);
        }
    }
}
