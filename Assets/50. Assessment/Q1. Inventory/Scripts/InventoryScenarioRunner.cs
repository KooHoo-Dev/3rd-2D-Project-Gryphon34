using UnityEngine;

// 1. 슬롯에 갯수를 더해도 슬롯의 추가하기 전에 return true;를 해버려서 갯수만 추가하고
// 슬롯을 추가하지 않고 빠져나와버린다. 따라서 return true를 빼고 슬롯을 추가하는 작업을 해주고 true를 반환해준다.

// 2. Compact 함수에서 반복문을 돌며 IsEmpty를 확인하는데
// 확인 후 제거를 하면 리스트의 특성상 인덱스가 앞으로 밀려 그 다음 반복문 실행시 하나를 놓치게 된다.
// 반복문을 역순으로 돌린다. Q3의 WaveRunner 인용
namespace Assessment.Inventory
{
    public class InventoryScenarioRunner : MonoBehaviour
    {
        [SerializeField] private int slotCapacity = 8;

        private void Start()
        {
            RunMergeScenario();
            RunCompactScenario();
        }

        private void RunMergeScenario()
        {
            ItemBag bag = new ItemBag(slotCapacity);

            bag.Add("Apple", 3);
            bag.Add("Apple", 3);
            bag.Add("Apple", 4);

            Debug.Log("[시나리오 A] Apple 을 3 + 3 + 4 = 10개 넣었다");
            PrintSlots(bag);
            Debug.Log($"    Apple 개수 {bag.CountOf("Apple")}");
        }

        private void RunCompactScenario()
        {
            ItemBag bag = new ItemBag(slotCapacity);

            bag.Add("Sword", 1);
            bag.Add("Shield", 1);
            bag.Add("Potion", 2);

            bag.Remove("Sword", 1);
            bag.Remove("Shield", 1);
            bag.Compact();

            bool breadAdded = bag.Add("Bread", 5);
            bool cheeseAdded = bag.Add("Cheese", 1);

            Debug.Log("[시나리오 B] Sword 와 Shield 를 뺀 뒤 빈 슬롯을 정리하고 새 아이템을 넣었다");
            Debug.Log($"    Bread 넣기 -> {breadAdded} / Cheese 넣기 -> {cheeseAdded}");
            PrintSlots(bag);
        }

        private void PrintSlots(ItemBag bag)
        {
            Debug.Log($"    슬롯 {bag.SlotCount}개");

            for (int i = 0; i < bag.SlotCount; i++)
            {
                Debug.Log($"      슬롯 {i} : {bag.GetSlot(i)}");
            }
        }
    }
}
