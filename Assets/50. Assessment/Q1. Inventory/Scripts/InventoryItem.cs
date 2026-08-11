namespace Assessment.Inventory
{
    // 중첩 가능한 아이템임에 유의
    public struct InventoryItem
    {
        public string Id;
        public int Count;

        public InventoryItem(string id, int count)
        {
            Id = id;
            Count = count;
        }

        public bool IsEmpty => Count <= 0;

        public override string ToString()
        {
            return $"{Id} x{Count}";
        }
    }
}
