using UnityEngine;
using System.Collections;

namespace Geometry
{
    public abstract class EnemyUnit : MonoBehaviour
    {
        //enum : 적 타입(동그라미, 네모, 세모)
        // float 변수: 최대_체력, 현재_체력, 이동속도, 기본_공격력, 사거리
        // bool 변수: 길이_막혔는가
        public enum EnemyType
        {
            Triangle,
            Circle,
            Square,
        }

        [SerializeField] protected float attackRange;
        [SerializeField] protected float attackDelay;
        [SerializeField] protected float attackCooldown;
        [SerializeField] protected float attackPower;
        [SerializeField] protected float maxHp;
        [SerializeField] protected float moveSpeed=2f;
        [SerializeField] protected float viewAngle = 180f;
        [SerializeField] protected Base playerBase;
        [SerializeField] protected int reward = 5;
        [SerializeField] protected SpriteRenderer enemyRenderer;

        protected IEnumerator nextStateCoroutine;
        public EnemyType enemyType;
        public PlayerUnit targetUnit;
        protected bool IsBlock;
        protected bool IsBase;
        public bool IsDead = false;
        public float current_Hp;
        private Transform hpBar;
        private float standardX;
        private float standardY;

        public GameObject deadState;
        protected Color a;

        protected virtual void Awake()
        {
            a = enemyRenderer.color;
            hpBar = transform.Find("helathBar");
            standardX = hpBar.transform.localScale.x;
            standardY = hpBar.transform.localScale.y;
            playerBase = GameObject.Find("Base").GetComponent<Base>();
            current_Hp = maxHp;
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

        public virtual void TakeDamage(float damage, PlayerManager.UnitType type, PlayerUnit attacker)
        {

        }

        protected void CheckPathBlock()
        {
            if (IsBlock == true || IsBase == true)
            {
                return;
            }
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);

            Vector3 forwardDir = (playerBase.transform.position - transform.position).normalized;

            foreach (Collider2D hit in hits)
            {
                if (hit.GetComponent<EnemyUnit>())
                {
                    continue;
                }
                Vector3 dirToTarget = (hit.transform.position - transform.position).normalized;
                if (Vector3.Angle(forwardDir, dirToTarget) < viewAngle / 2)
                {
                    if (hit.GetComponent<PlayerUnit>())
                    {
                        IsBlock = true;
                        targetUnit = hit.GetComponent<PlayerUnit>();
                        return;
                    }
                    else if (hit.GetComponent<Base>())
                    {
                        IsBase = true;
                        return;
                    }
                }
            }
        }

        public void DeadState()
        {
            a.a = 0f;
            enemyRenderer.color = a;
            deadState.SetActive(true);
        }
    }
}