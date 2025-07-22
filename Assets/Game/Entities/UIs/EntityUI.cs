using Asce.Game.UIs;
using Asce.Game.UIs.Creatures;
using Asce.Managers;
using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities
{
    public class EntityUI : GameComponent, IEntityUI, IHasOwner<Entity>
    {
        [SerializeField, Readonly] private Entity _owner;
        [SerializeField] protected UIEntityCanvas _mainUI;

        [Header("Config")]
        [SerializeField] protected bool _isHideOnDead = true;
        [SerializeField] protected Cooldown _hideCooldown = new(10f);

        public Entity Owner
        {
            get => _owner;
            set => _owner = value;
        }
        public UIEntityCanvas MainUI => _mainUI;

        public virtual bool IsHideOnDead
        {
            get => _isHideOnDead;
            set => _isHideOnDead = value;
        }

        protected override void RefReset()
        {
            base.RefReset();
            this.LoadComponent(out _owner);
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {
            _hideCooldown.Update(Time.deltaTime);
        }

        public virtual void Register()
        {
            if (Owner == null) return;

            _hideCooldown.OnCompleted += (sender) => { if (MainUI != null) MainUI.Hide(); };
            _hideCooldown.OnTimeReset += (sender) => { if (MainUI != null) MainUI.Show(); };

            if (MainUI != null)
            {
                MainUI.SetVerticalPosition(Owner.Status.Height + 0.5f);
                MainUI.BaseName = (Owner.Information != null) ? Owner.Information.Name : string.Empty;
                MainUI.ResetBaseName();
                MainUI.Hide();

                Owner.Status.OnDeath += Status_OnDeath;
                Owner.Status.OnRevive += Status_OnRevive;
                Owner.Status.OnHeightChanged += Status_OnHeightChanged;
            }
        }

        protected virtual void Status_OnDeath(object sender)
        {
            if (IsHideOnDead) MainUI.Hide();
        }
        protected virtual void Status_OnRevive(object sender)
        {
            MainUI.Hide();
        }
        protected virtual void Status_OnHeightChanged(object sender, ValueChangedEventArgs args)
        {
            MainUI.SetVerticalPosition(Owner.Status.Height + 0.5f);
        }
    }
}