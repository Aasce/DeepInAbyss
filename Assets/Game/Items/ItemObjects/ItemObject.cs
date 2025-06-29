using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Items
{
    public class ItemObject : MonoBehaviour
    {
        // Ref
        [SerializeField, Readonly] protected ItemObjectView _view;

        [SerializeField] protected SO_ItemInformation _information;

        [Space]
        [SerializeField] protected bool _autoDespawn = true;
        [SerializeField] protected Cooldown _despawnCooldown = new(30f);

        public ItemObjectView View => _view;
        public SO_ItemInformation Information
        {
            get => _information;
            protected set => _information = value;
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
        }
        protected virtual void Awake() { }
        protected virtual void Start() { }


        public virtual void SetItem(SO_ItemInformation information)
        {
            if (information == null) return;
            Information = information;

            if (View != null)
            {
                View.SetIcon(Information.Icon);
            }
        }
    }
}
