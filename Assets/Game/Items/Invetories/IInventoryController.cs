namespace Asce.Game.Inventories
{
    public interface IInventoryController
    {
        public Inventory Inventory { get; }
        public void Drop(int index, int quantity = -1);
    }
}