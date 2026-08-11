using System;
using System.Collections.Generic;

namespace Assessment.Inventory
{
    // 아이템 한 종류가 슬롯 하나를 쓴다. 이미 있는 아이템을 또 넣으면 그 슬롯의 개수만 늘어난다.
    public class ItemBag
    {
        private readonly List<InventoryItem> slots = new List<InventoryItem>();

        private readonly int slotCapacity;

        public ItemBag(int slotCapacity)
        {
            this.slotCapacity = slotCapacity;
        }

        public int SlotCount => slots.Count;

        public InventoryItem GetSlot(int index)
        {
            return slots[index];
        }

        // 새 슬롯이 필요한데 빈 칸이 없으면 false.
        public bool Add(string id, int count)
        {
            if (slots.Count >= slotCapacity) return false;
            
            for (int i = 0; i < slots.Count; i++)
            {
                InventoryItem slot = slots[i];
                if (slot.Id != id) continue;
                
                slot.Count += count;
                // 이게 정답이였슨....
                // slots[i] = slot;
            }

            slots.Add(new InventoryItem(id, count));

            return true;
        }

        // 실제로 빠진 개수를 돌려준다.
        // 개수가 0이 되어도 슬롯은 남긴다. 목록에서 치우는 것은 Compact 의 몫이다.
        public int Remove(string id, int count)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                InventoryItem slot = slots[i];
                if (slot.Id != id) continue;

                int taken = Math.Min(slot.Count, count);
                slot.Count -= taken;
                slots[i] = slot;

                return taken;
            }

            return 0;
        }

        // 개수가 0이 된 슬롯을 목록에서 치우고 뒤 슬롯을 앞으로 당긴다.
        public void Compact()
        {
            for (int i = slots.Count-1; i >= 0; i--)
            {
                // 지금 Sword가 빠지는 건 slots의 0번에 있어서 빠지고
                // 그 다음은 Shield인데 이거는 slots의 0번에 있는데 이미 반복문이 1번으로 가서
                // 안빠짐. 그러면 빠지면 다시 0번을 살펴야 함.
                if (slots[i].IsEmpty)
                {
                    
                    slots.RemoveAt(i);
                }
            }
        }

        public int CountOf(string id)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].Id == id) return slots[i].Count;
            }

            return 0;
        }
    }
}
