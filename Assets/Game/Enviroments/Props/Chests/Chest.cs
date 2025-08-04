using Asce.Game.Entities;
using Asce.Game.Entities.Characters;
using Asce.Game.Inventories;
using Asce.Game.Items;
using Asce.Game.SaveLoads;
using Asce.Game.UIs;
using Asce.Game.UIs.Chests;
using Asce.Managers.Attributes;
using Asce.Managers.SaveLoads;
using Asce.Managers.Utils;
using System.Threading.Tasks;
using UnityEngine;

namespace Asce.Game.Enviroments
{
    public class Chest : InteractiveObject, IEnviromentComponent, IInteractableObject, IUniqueIdentifiable, IInventoryController, IReceiveData<bool>
    {
        // Ref
        [SerializeField, Readonly] protected string _id = string.Empty;
        [SerializeField, Readonly] protected Animator _animator;

        [Space]
        [SerializeField] protected SO_DefaultInventoryItems _defaultItems;
        [SerializeField] protected Inventory _inventory = new(15);
        [SerializeField, Readonly] protected bool _isOpened = false;
        [SerializeField] protected bool _isLoaded = false;

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

        protected virtual void Start()
        {
            _ = this.Load();
        }

        public virtual void Drop(int index, int quantity = -1)
        {
            Item dropped = _inventory.RemoveAt(index, quantity);
            if (dropped.IsNull()) return;

            Vector2 position = (Vector2)transform.position + Vector2.up;
            ItemObject itemObject = ItemObjectsManager.Instance.Spawn(dropped, position);
            if (itemObject == null) return;
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
            base.Interact(interactor);
        }

        public override void Focus()
        {
            base.Focus();
        }

        public override void Unfocus()
        {
            base.Unfocus();
        }

        protected virtual async Task Load()
        {
            await SaveLoadManager.Instance.WaitUntilLoadedAsync();
            if (_isLoaded) return;
            if (_defaultItems == null) return;
            for(int i = 0; i < _defaultItems.Items.Count; i++)
            {
                if (i >= Inventory.SlotCount) break;

                InventoryItemContainer container = _defaultItems.Items[i];
                Item item = container.CreateItem();
                if (item.IsNull()) continue;

                Inventory.AddAt(item, i);
            }
        }

        void IReceiveData<bool>.Receive(bool data)
        {
            _isLoaded = true;
        }
    }
}
