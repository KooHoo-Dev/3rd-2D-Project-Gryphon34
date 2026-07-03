using UnityEngine;
using System.Collections;

namespace Geometry
{
    public abstract class PlayerUnit : MonoBehaviour
    {
        [SerializeField] protected float attackRange;
        [SerializeField] protected float scanCooldown;
        [SerializeField] protected float attackCooldown;
        [SerializeField] protected float attackDelay;
        [SerializeField] protected float attackPower;
        [SerializeField] protected float maxHp;
        [SerializeField] protected float maxMorale;
        [SerializeField] protected LayerMask targetLayer;
        [SerializeField] protected SpriteRenderer unitRenderer;
        [SerializeField] protected SpriteRenderer fordRenderer;

        protected IEnumerator nextStateCoroutine;
        public PlayerManager.UnitType myType;
        public EnemyUnit targetEnemy;
        public float current_Hp;
        public float current_Morale;

        public GameObject deadState;
        protected Color a;

        private Transform hpBar;
        private float standardX;
        private float standardY;
        protected bool isDead = false;

        protected virtual void Awake()
        {
            fordRenderer.enabled = true;
            a= unitRenderer.color;
            hpBar = transform.Find("helathBar");
            standardX = hpBar.transform.localScale.x;
            standardY = hpBar.transform.localScale.y;
            current_Hp = maxHp;
            current_Morale = maxMorale;
        }
        private void Start()
        {
            deadState.SetActive(false);
        }

        public void updateHpBar()
        {
            if (hpBar != null)
            {
                float hpRatio = current_Hp / maxHp;
                hpBar.transform.localScale = new Vector3(hpRatio* standardX, standardY, 1f);
            }
        }


        public virtual void TakeDamage(float damage, EnemyUnit.EnemyType type)
        {

        }

        public void DeadState()
        {
            a.a = 0f;
            unitRenderer.color = a;
            Debug.Log("애니메이션 재생");
            deadState.SetActive(true);
        }
    }
}
