using Asce.Game.UIs.Creatures;
using Asce.Game.UIs.Ores;
using Asce.Managers;
using UnityEngine;

namespace Asce.Game.Entities.Ores
{
    public class OreUI : EntityUI, IHasOwner<Ore>, IEntityUI
    {
        public new Ore Owner 
        { 
            get => base.Owner as Ore; 
            set => base.Owner = value; 
        }
        public new UIOreCanvas MainUI => base.MainUI as UIOreCanvas;

        public override void Register()
        {
            if (Owner == null) return;
            if (MainUI != null)
            {
                MainUI.HealthBar.SetStat(Owner.Stats.HealthGroup.Health, Owner.Stats.DefenseGroup.Shield);
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