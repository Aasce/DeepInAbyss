using Asce.Game.Entities;
using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Items
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class ItemObject : MonoBehaviour
    {
        // Ref
        [SerializeField, Readonly] protected ItemObjectView _view;
        [SerializeField, Readonly] protected Rigidbody2D _rigidbody;
        [SerializeField, Readonly] protected Collider2D _collider;

        [SerializeField] protected SO_ItemInformation _information;
        [SerializeField] protected int _quantity = 1;

        [Space]
        [SerializeField] protected bool _pickable = false;
        [SerializeField] protected bool _isPicked = false;
        [SerializeField] protected bool _autoDespawn = true;
        [SerializeField] protected Cooldown _despawnCooldown = new(30f);

        public ItemObjectView View => _view;
        public Rigidbody2D Rigidbody => _rigidbody;
        public Collider2D Collider => _collider;

        public SO_ItemInformation Information
        {
            get => _information;
            protected set => _information = value;
        }

        public int Quantity
        {
            get => _quantity;
            set => _quantity = value;
        }

        public bool Pickable
        {
            get => _pickable;
            set => _pickable = value;
        }
        public bool IsPicked
        {
            get => _isPicked;
            protected set => _isPicked = value;
        }
        public bool AutoDespawn
        {
            get => _autoDespawn;
            set => _autoDespawn = value;
        }
        public Cooldown DespawnCooldown => _despawnCooldown;

        protected virtual void Reset()
        {
            this.LoadComponent(out _view);
            this.LoadComponent(out _rigidbody);
            this.LoadComponent(out _collider);
        }
        protected virtual void Awake() { }
        protected virtual void Start() { }

        public virtual void Refresh()
        {
            this.DespawnCooldown.Reset();
            this.IsPicked = false;
        }

        public virtual void SetItem(SO_ItemInformation information)
        {
            if (information == null) return;
            Information = information;
            this.name = information.Name;

            if (View != null)
            {
                View.SetIcon(Information.Icon);
            }
        }

        public virtual void PickedBy(ICreature picker)
        {
            IsPicked = true;
        }
    }
}
