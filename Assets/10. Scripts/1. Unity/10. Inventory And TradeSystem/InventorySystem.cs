using Jay;
//using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

namespace Study_Inventory
{
    // 모든 인벤토리를 관리하고, 서칭할 수 있는 역할의 객체
    // PS : System이나 Manager가 해당 역할을 주로 담당합니다.
    //     EnemyManager     : 모든 적 개체를 관리(생성, 삭제, 조회, 변경)하는 객체
    //     ItemManager      : 모든 아이템 개체를 관리(생성, 삭제, 조회, 변경)하는 객체
    //     PlayerManager    : 모든 플레이어 개체를 관리(생성, 삭제, 조회, 변경)하는 객체
    //     VFXManager       : 모든 VFX 개체를 관리(생성, 삭제, 조회, 변경)하는 객체

    // - 보통은 정보 조회가 메인을 담당합니다.
    // - 보통은 싱글톤 형태로 선언이 됩니다.

    // 순수 선형탐색으로 맹글어 보겠습니다
    public class InventorySystem : SingletonBase<InventorySystem>
    {
        private List<Inventory> Inventories { get; set; } = new List<Inventory>();

        // 유니티에서 관리 개체를 등록하는 방식은 크게 두가지가 있는데
        // 1. System이나 Manager가 찾거나 생성하는 것
        // 2. 관리 개체가 스스로 등록하는것
        // 이번에는 스스로 등록하는것을 스터디해봅시다

        public void Register(Inventory inventory)
        {
            Inventories.Add(inventory);
        }

        public void Remove(Inventory inventory)
        {
            Inventories.Remove(inventory);
        }

        // 인벤토리의 이름으로 조회를 합니다
        public Inventory GetInventoryOrNull(string targetName)
        {
            for (int i = 0; i < Inventories.Count; i++)
            {
                if (Inventories[i].name.Equals(targetName))
                    return Inventories[i];
            }

            return null;
        }

        // Slot을 이용해서 조회를 합니다. 함수 오버로딩을 사용합니다.
        // - 함수 오버로딩 : 같은 함수이름을 갖고 있지만, 매개변수가 달라서
        //                  컴파일이 허용되는 문법적 규칙
        public Inventory GetInventoryOrNull(InventorySlot slot)
        {
            // 모든 인벤토리에서 slot이 어떠한 인벤토리에 있는지 검색을 한다

            for (int i = 0; i < Inventories.Count; ++i)
            {
                if (Inventories[i].IsIn(slot)) return Inventories[i];
            }

            return null;
        }

    }
}


