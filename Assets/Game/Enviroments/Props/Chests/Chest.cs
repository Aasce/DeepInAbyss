using Asce.Game.Entities;
using Asce.Game.Entities.Characters;
using Asce.Game.Inventories;
using Asce.Game.Items;
using Asce.Game.UIs;
using Asce.Game.UIs.Chests;
using Asce.Managers.Attributes;
using Asce.Managers.SaveLoads;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Enviroments
{
    public class Chest : InteractiveObject, IEnviromentComponent, IInteractableObject, IUniqueIdentifiable, IInventoryController
    {
        // Ref
        [SerializeField, Readonly] protected string _id = string.Empty;
        [SerializeField, Readonly] protected Animator _animator;

        [Space]
        [SerializeField] protected Inventory _inventory = new(15);
        [SerializeField, Readonly] protected bool _isOpened = false;

        protected ICreature _opener;

        public string ID => _id;

        public Animator Animator => _animator;
        public Inventory Inventory => _inventory;

        public ICreature Opener => _opener;
        public bool IsOpened
        {
            get => _isOpened;
            set
            {
                _isOpened = value;

                if (Animator == null) return;
                Animator.SetBool("IsOpened", _isOpened);
            }
        }

        protected override void RefReset()
        {
            base.RefReset();
            this.LoadComponent(out _animator);
        }

        public virtual void Drop(int index, int quantity = -1)
        {
            ItemStack dropStack = _inventory.RemoveAt(index, quantity);
            SO_ItemInformation info = dropStack.GetItemInfo();
            if (info == null) return;

            Vector2 position = (Vector2)transform.position + Vector2.up;
            ItemObject itemObject = ItemObjectsManager.Instance.Spawn(info.Name, position);
            if (itemObject == null) return;

            itemObject.Quantity = dropStack.Quantity;
        }

        public override void Interact(GameObject interactor)
        {
            if (interactor.TryGetComponent(out ICreature creature))
            {
                if (creature is IHasInteraction<CharacterInteraction> hasInteraction)
                {
                    _opener = creature;
                }
            }

            if (!_opener.IsControlByPlayer()) return;
            UIChestWindow window = UIScreenCanvasManager.Instance.WindowsController.GetWindow<UIChestWindow>();
            if (window == null) return;

            window.SetChest(this);
            window.Show();
        }

        public override void Focus()
        {
            base.Focus();
        }

        public override void Unfocus()
        {
            base.Unfocus();
        }
    }
}
