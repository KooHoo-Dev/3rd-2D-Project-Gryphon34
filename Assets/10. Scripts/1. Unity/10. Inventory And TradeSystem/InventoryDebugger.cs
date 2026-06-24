using UnityEngine;
using Study.Utilities;
using UnityEngine.InputSystem;

namespace Study_Inventory
{
    public class InventoryDebugger : MonoBehaviour
    {
        public Inventory targetInventory;
        public Key fillKey = Key.Space;

        public InventoryItem testItem;

        private void Awake()
        {
            targetInventory = GetComponent<Inventory>();
        }

        private void Update()
        {
            // 키가 입력이 되지 않았으면 Update를 종료
            if (SimpleInput.GetKeyDown(fillKey) == false) return;

            // targetInventory의 절반 정도를 특정 아이템들로 채워줍니다

            InventorySlot[,] grid = targetInventory.SlotGrid;
            int halfRow = (targetInventory.SizeRow / 2) +1;
            int halfColumn = (targetInventory.SizeColumn / 2) + 1;

            InventoryItemDB db = InventoryItemDB.Instance;

            for(int i= 0; i < halfRow; ++i)
            {
                for(int j = 0; j < halfColumn; ++j)
                {
                    int randIndex = Random.Range(0, db.Count);
                    grid[i, j].SetItem(db.GetItem(randIndex));
                }
            }
        }

    }
}