using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Study_Inventory
{
    public class InventorySlotDebugger : MonoBehaviour
    {
        public InventorySlot Slot;
        public InventoryItem TestItem;

        private void Start()
        {
            Slot.SetItem(null);
        }

        private void Update()
        {
            if (SimpleInput.GetKeyDown(Key.Digit1))
            {
                Slot.SetItem(TestItem);
            }

            if (SimpleInput.GetKeyDown(Key.Digit2))
            {
                Slot.SetItem(null);
            }
        }
    }

}
