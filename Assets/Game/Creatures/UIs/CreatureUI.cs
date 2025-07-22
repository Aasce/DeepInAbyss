using Asce.Game.UIs;
using Asce.Game.UIs.Creatures;
using Asce.Managers;
using UnityEngine;

namespace Asce.Game.Entities
{
    public class CreatureUI : EntityUI, IHasOwner<Creature>, IEntityUI
    {
        public new Creature Owner 
        { 
            get => base.Owner as Creature;
            set => base.Owner = value;
        }
        public new UICreatureCanvas MainUI => base.MainUI as UICreatureCanvas;

        public override void Register()
        {
            if (Owner == null) return;

            if (MainUI != null)
            {
                MainUI.HealthBar.SetStat(Owner.Stats.HealthGroup.Health, Owner.Stats.DefenseGroup.Shield);
                MainUI.SetUIForPlayer(Owner);
            }

            Owner.Stats.HealthGroup.Health.OnValueChanged += Stats_OnValueChanged;
            Owner.Stats.HealthGroup.Health.OnCurrentValueChanged += Stats_OnValueChanged;

            Owner.Stats.DefenseGroup.Shield.OnValueChanged += Stats_OnValueChanged;
            Owner.Stats.DefenseGroup.Shield.OnCurrentValueChanged += Stats_OnValueChanged;

            base.Register();
        }

        protected virtual void Stats_OnValueChanged(object sender, ValueChangedEventArgs args)
        {
            _hideCooldown.Reset();
        }
    }
}