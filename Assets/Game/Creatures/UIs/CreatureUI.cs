using Asce.Game.UIs;
using Asce.Game.UIs.Creatures;
using Asce.Managers;
using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities
{
    public class CreatureUI : MonoBehaviour, IHasOwner<Creature>, ICreatureUI
    {
        [SerializeField, Readonly] private Creature _owner;

        [SerializeField] protected UICreatureCanvas _mainUI;

        [Header("Config")]
        [SerializeField] protected bool _isHideOnDead = true;
        [SerializeField] protected Cooldown _hideCooldown = new(10f);

        public Creature Owner 
        { 
            get => _owner;
            set => _owner = value;
        }
        public UICreatureCanvas MainUI => _mainUI;


        public virtual bool IsHideOnDead
        {
            get => _isHideOnDead;
            set => _isHideOnDead = value;
        }

        protected virtual void Reset()
        {
            if (this.LoadComponent(out _owner))
            {
                Owner.UI = this;
            }
        }

        protected virtual void Awake() { }
        protected virtual void Start()
        {
            this.Register();
            _hideCooldown.OnCompleted += (sender) => { if (MainUI != null) MainUI.Hide(); };
            _hideCooldown.OnTimeReset += (sender) => { if (MainUI != null) MainUI.Show(); };
        }

        protected virtual void Update()
        {
            _hideCooldown.Update(Time.deltaTime);
        }


        protected virtual void Register()
        {
            if (MainUI != null)
            {
                MainUI.SetVerticalPosition(Owner.Status.Height + 0.5f);
                MainUI.BaseName = (Owner.Information != null) ? Owner.Information.Name : string.Empty;
                MainUI.HealthBar.SetStat(Owner.Stats.HealthGroup.Health, Owner.Stats.DefenseGroup.Shield);
                MainUI.SetUIForPlayer(Owner);
                
                Owner.Status.OnDeath += Status_OnDeath;
                Owner.Status.OnRevive += Status_OnRevive;
                Owner.Status.OnHeightChanged += Status_OnHeightChanged;
            }

            Owner.Stats.HealthGroup.Health.OnCurrentValueChanged += Stats_OnValueChanged;
            Owner.Stats.HealthGroup.Health.OnValueChanged += Stats_OnValueChanged;
            Owner.Stats.DefenseGroup.Shield.OnValueChanged += Stats_OnValueChanged;
        }

        protected virtual void Status_OnDeath(object sender)
        {
            if (IsHideOnDead) MainUI.Hide();
        }
        protected virtual void Status_OnRevive(object sender)
        {
            _hideCooldown.Reset();
        }
        protected virtual void Status_OnHeightChanged(object sender, ValueChangedEventArgs args)
        {
            MainUI.SetVerticalPosition(Owner.Status.Height + 0.5f);
        }
        protected virtual void Stats_OnValueChanged(object sender, ValueChangedEventArgs args)
        {
            _hideCooldown.Reset();
        }

    }
}