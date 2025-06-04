using Asce.Game.Stats;
using Asce.Game.UIs.Stats;
using Asce.Managers.UIs;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.UIs.Stats
{
    public class UIResourceStatsInfoController : UIObject
    {
        [SerializeField] protected UIHealthBarGroup _uiHealthGroup;
        [SerializeField] protected UITimeBasedResourceStatBarGroup _uiStaminaGroup;
                                                          
        [Header("Sustenance Stats")]                      
        [SerializeField] protected UITimeBasedResourceStatBarGroup _uiHungerGroup;
        [SerializeField] protected UITimeBasedResourceStatBarGroup _uiThirstGroup;
        [SerializeField] protected UITimeBasedResourceStatBarGroup _uiBreathGroup;

        [Space]
        [SerializeField] protected Cooldown _hideBreathCooldown = new(10f);

        protected virtual void Start()
        {
            if (_uiBreathGroup != null) _uiBreathGroup.StatBar.OnStatTargetChanged += UIBreath_OnStatTargetChanged;
        }

        protected virtual void Update()
        {
            _hideBreathCooldown.Update(Time.deltaTime);
            if (_hideBreathCooldown.IsComplete)
            {
                if (_uiBreathGroup != null) _uiBreathGroup.Hide();
            }
        }

        public virtual void SetStats(TimeBasedResourceStat health, ResourceStat shield, TimeBasedResourceStat stamina, TimeBasedResourceStat hunger, TimeBasedResourceStat thirst, TimeBasedResourceStat breath)
        {
            _uiHealthGroup.StatBar.SetStat(health, shield);
            _uiStaminaGroup.StatBar.SetStat(stamina);

            _uiHungerGroup.StatBar.SetStat(hunger);
            _uiThirstGroup.StatBar.SetStat(thirst);
            _uiBreathGroup.StatBar.SetStat(breath);

        }

        public virtual void SetStats(TimeBasedResourceStat health, ResourceStat shield, TimeBasedResourceStat stamina, SustenanceGroupStats sustenance)
        {
            if (sustenance == null)
                this.SetStats(health, shield, stamina, null, null, null);
            this.SetStats(health, shield, stamina, sustenance.Hunger, sustenance.Thirst, sustenance.Breath);
        }


        private void UIBreath_OnStatTargetChanged(object sender, StatTargetChangedEventArgs<TimeBasedResourceStat> args)
        {
            if (args.OldStat != null)
            {
                args.OldStat.OnValueChanged -= BreathStat_OnValueChanged;
                args.OldStat.OnCurrentValueChanged -= BreathStat_OnValueChanged;
            }

            if (args.NewStat != null)
            {
                args.NewStat.OnValueChanged += BreathStat_OnValueChanged;
                args.NewStat.OnCurrentValueChanged += BreathStat_OnValueChanged;
            }
        }

        private void BreathStat_OnValueChanged(object sender, Managers.ValueChangedEventArgs args) 
        { 
            _hideBreathCooldown.Reset();
            if (_uiBreathGroup != null) _uiBreathGroup.Show();
        }
    }
}