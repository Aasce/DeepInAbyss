using Asce.Game.Entities;
using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.UIs.Inventories
{
    public class UIInventoryWindow : UIWindow
    {
        [SerializeField, Readonly] protected UIInventory _inventory;
        protected ICreature _creature;

        protected override void RefReset()
        {
            base.RefReset();
            this.LoadComponent(out _inventory);
        }

        public void SetCreature(ICreature creature)
        {
            if (_creature == creature) return;

            this.Unregister();
            _creature = creature;
            this.Register();
        }

        protected virtual void Register() 
        {
            if (_creature == null) return;
            if (_inventory != null)
                _inventory.SetInventory(_creature.Inventory);
        }
        protected virtual void Unregister() 
        {
            if (_creature == null) return;
        }
    }
}
