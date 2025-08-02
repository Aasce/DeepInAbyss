using Asce.Game.Entities;
using Asce.Game.Inventories;
using Asce.Game.Items;
using Asce.Game.UIs;
using Asce.Game.UIs.Shops;
using Asce.Managers.Attributes;
using Asce.Managers.SaveLoads;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Enviroments
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Shop : InteractiveObject, IUniqueIdentifiable, IEnviromentComponent, IInteractableObject
    {
        [SerializeField, Readonly] protected string _id;

        // Ref
        [SerializeField, Readonly] protected BoxCollider2D _collider;

        [Space]
        [SerializeField] protected SO_ShopItems _shopItems;

        public string ID => _id;
        public BoxCollider2D Collider => _collider;

        public SO_ShopItems ShopItems => _shopItems;


        protected override void RefReset()
        {
            base.RefReset();
            this.LoadComponent(out _collider);
        }

        protected virtual void Start()
        {
            if (_shopItems == null) IsInteractable = false;
            else IsInteractable = true;
        }

        public override void Interact(GameObject interactor)
        {
            if (!interactor.TryGetComponent(out ICreature creature)) return;
            if (creature.Inventory is not IInventoryController inventoryController) return;

            UIShopWindow window = UIScreenCanvasManager.Instance.WindowsController.GetWindow<UIShopWindow>();
            if (window == null) return;

            window.SetShop(this, inventoryController);
            window.Show();
            base.Interact(interactor);
        }
    }
}
