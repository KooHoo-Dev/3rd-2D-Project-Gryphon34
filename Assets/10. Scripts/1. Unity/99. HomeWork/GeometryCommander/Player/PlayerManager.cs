using Jay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Geometry
{
    public class PlayerManager : SingletonBase<PlayerManager>
    {
        // int 변수 : 현재_자원, 최대_인구수, 현재_인구수
        // GameObject: 현재_선택된_포드
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        public enum UnitType
        {
            Triangle = 5,
            Circle = 7,
            Square = 9,
            Support = 12,
        }

        public enum CoolDown
        {
            triangleCooldown = 5,
            circleCooldown = 7,
            squareCooldown = 9,
            supportCooldown = 15,
        }

        private Dictionary<UnitType, bool> IsCooldownDic = new Dictionary<UnitType, bool>();
        public Dictionary <UnitType, float> UnitCooldownDic = new Dictionary<UnitType, float>();

        [Header("자원 및 인구수 조절")]
        [field: SerializeField] public int maxCost { get; private set; }
        [field: SerializeField] public int currentCost { get; private set; }
        [field: SerializeField] public int maxSupply { get; private set; }
        [field: SerializeField] public int currentSupply { get; private set; }

        [Header("개체 별 자원 및 인구수 조절")]
        [SerializeField] private int TriangleCost = 10;
        [SerializeField] private int CircleCost = 15;
        [SerializeField] private int SquareCost = 20;
        [SerializeField] private int SupportCost = 25;
        [SerializeField] private int TriangleSupply = 2;
        [SerializeField] private int CircleSupply = 4;
        [SerializeField] private int SquareSupply = 6;

        [Header("자원 획득")]
        [SerializeField] private int costPerSecond = 1;

        private void Start()
        {
            StartCoroutine(PassiveCostRooutine());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator PassiveCostRooutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                AddCost(costPerSecond);
            }
        }

        public void AddCost(int amount)
        {
            currentCost += amount;
            if (currentCost > maxCost) currentCost = maxCost;
        }

        public bool CanAfford(UnitType type)
        {
            if (type == UnitType.Support) return true;
            if (currentCost >= GetCost(type) && (maxSupply - currentSupply )>= GetSupply(type)) return true;

            return false;
        }

        public void UseCost(UnitType type)
        {
            if (CanAfford(type))
            {
                int cost = GetCost(type);
                int supply = GetSupply(type);
                currentSupply += supply;
                currentCost -= cost;
                Debug.Log($"소비 자원 : {cost}, 현재 자원 : {currentCost}");
                Debug.Log($"추가된 인원 : {supply}, 현재 인구수 : {currentSupply}, 남은 인구수 : {maxSupply - currentSupply}");
                StartCoroutine(UnitCoolDown(type));
            }
        }

        public int GetCost(UnitType type)
        {
            switch (type)
            {
                case UnitType.Triangle:
                    return TriangleCost;
                case UnitType.Circle:
                    return CircleCost;
                case UnitType.Square:
                    return SquareCost;
                case UnitType.Support:
                    return SupportCost;
                default:
                    return 0;
            }
        }

        public int GetSupply(UnitType type)
        {
            switch (type)
            {
                case UnitType.Triangle:
                    return TriangleSupply;
                case UnitType.Circle:
                    return CircleSupply;
                case UnitType.Square:
                    return SquareSupply;
                default:
                    return 0;
            }
        }

        public IEnumerator UnitCoolDown(UnitType type)
        {
            IsCooldownDic[type] = true;
            float cooldownSec=0f;
            float currentTime = 0f;

            switch (type)
            {
                case UnitType.Triangle:
                    cooldownSec = (int)CoolDown.triangleCooldown;
                    break;
                case UnitType.Circle:
                    cooldownSec = (int)CoolDown.circleCooldown;
                    break;
                case UnitType.Square:
                    cooldownSec = (int)CoolDown.squareCooldown;
                    break;
                case UnitType.Support:
                    cooldownSec = (int)CoolDown.supportCooldown;
                    break;
            }

            while (true)
            {
                if (currentTime >= cooldownSec) break;
                currentTime += Time.deltaTime;
                UnitCooldownDic[type]=currentTime/ cooldownSec;
                yield return null;
            }
            IsCooldownDic[type] = false;
            UnitCooldownDic[type] = 1f;
        }

        public bool IsOnCooldown(UnitType type)
        {
            if (IsCooldownDic.TryGetValue(type,out bool isCool))
            {
                return isCool;
            }
            return false;
        }

        public void Dead(PlayerUnit unit)
        {
            int supply = GetSupply(unit.myType);
            currentSupply-=supply;
            Destroy(unit.gameObject);
        }

        public void Dead(EnemyUnit unit)
        {
            Destroy(unit.gameObject);
        }
    }
}