namespace Game.Scripts.InventorySystem
{
    public interface IItem
    {
        public ItemConfig Config { get; }

        public void SetConfig(ItemConfig item);
    }
}