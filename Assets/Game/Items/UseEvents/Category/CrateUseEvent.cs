using UnityEngine;

namespace Asce.Game.Items
{
	public class CrateUseEvent : UseEvent
    {
        public override bool OnUse(object sender, UseEventArgs args)
        {
			if (args == null) return false;
			if (args.User == null) return false;
			if (args.User.Inventory == null) return false;
			
			int count = ItemObjectsManager.Instance.ItemData.Data.Count;
			if (count <= 0) return false;
			
			int randomIndex = UnityEngine.Random.Range(0, count);
			ItemContainer container = ItemObjectsManager.Instance.ItemData.Data[randomIndex];
			if (container == null || container.Information == null) return false;
			
			Item newItem = new(container.Information);
			newItem.SetQuantity(1);
			newItem.SetDurability(container.Information.GetMaxDurability());
			
			args.User.Inventory.Inventory.AddItem(newItem);
			
			return true;			
		}
	}	
}