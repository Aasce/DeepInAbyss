using Asce.Game.Inventories;

namespace Asce.Game.UIs.Inventories
{
    public interface IUIInventory
    {
        public IInventoryController Controller { get; }
    }
}
