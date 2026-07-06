using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

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
        private Transform moraleBar;
        private float hpStandardX;
        private float moraleStandardX;
        private float hpStandardY;
        private float moraleStandardY;
        protected bool isDead = false;

        protected virtual void Awake()
        {
            unitRenderer.enabled = false;
            fordRenderer.enabled = true;
            a= unitRenderer.color;
            hpBar = transform.Find("helathBar");
            moraleBar = transform.Find("moraleBar");
            hpStandardX = hpBar.transform.localScale.x;
            moraleStandardX = moraleBar.transform.localScale.x;
            hpStandardY = hpBar.transform.localScale.y;
            moraleStandardY = moraleBar.transform.localScale.y;
            current_Hp = maxHp;
            current_Morale = maxMorale;
        }
        protected virtual void Start()
        {
            deadState.SetActive(false);
        }

        protected virtual void Update()
        {
            if (current_Morale >= maxMorale)
            {
                current_Morale=maxMorale;
            }

            if ((current_Hp <= 0 || current_Morale <= 0) && !isDead)
            {
                isDead = true;
                StopAllCoroutines();
                DeadState();
            }
            updateHpBar();
            updateMoraleBar();
        }

        public void updateHpBar()
        {
            if (hpBar != null)
            {
                float hpRatio = current_Hp / maxHp;
                hpBar.transform.localScale = new Vector3(hpRatio* hpStandardX, hpStandardY, 1f);
            }
        }

        public void updateMoraleBar()
        {
            if(moraleBar != null)
            {
                float moraleRatio = current_Morale / maxMorale;
                moraleBar.transform.localScale = new Vector3(moraleRatio * moraleStandardX, moraleStandardY, 1f);
            }
        }

        public virtual void TakeDamage(float damage, EnemyUnit.EnemyType type)
        {

        }

        protected virtual void DeadState()
        {
            a.a = 0f;
            unitRenderer.color = a;
            Debug.Log("애니메이션 재생");
            deadState.SetActive(true);
        }
    }
}
