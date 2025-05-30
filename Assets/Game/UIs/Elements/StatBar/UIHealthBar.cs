using Asce.Game.Stats;
using Asce.Managers;
using Asce.Managers.UIs;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs
{
    public class UIHealthBar : UIObject
    {
        [SerializeField] protected Slider _healthSlider;
        [SerializeField] protected Slider _shieldSlider;

        protected ResourceStat _healthStat;
        protected ResourceStat _shieldStat;

        
        public Slider HealthSlider => _healthSlider;
        public Slider ShieldSlider => _shieldSlider;

        public ResourceStat HealthStat => _healthStat;
        public ResourceStat ShieldStat => _shieldStat;


        public void SetStat(ResourceStat healthStat, ResourceStat shieldStat)
        {
            this.Unregister();

            this._healthStat = healthStat;
            this._shieldStat = shieldStat;

            this.Register();
        }

        protected virtual void Register()
        {
            if (HealthStat == null) return;
            if (ShieldStat == null) return;

            this.SetMaxValue(HealthStat.Value + ShieldStat.CurrentValue);
            HealthSlider.value = HealthStat.CurrentValue;
            ShieldSlider.value = HealthStat.CurrentValue + ShieldStat.CurrentValue;

            HealthStat.OnValueChanged += HealthStat_OnValueChanged;
            HealthStat.OnCurrentValueChanged += HealthStat_OnCurrentValueChanged;

            ShieldStat.OnCurrentValueChanged += ShieldStat_OnCurrentValueChanged;
        }

        protected virtual void Unregister()
        {
            if (HealthStat == null) return;
            if (ShieldStat == null) return;

            HealthStat.OnValueChanged -= HealthStat_OnValueChanged;
            HealthStat.OnCurrentValueChanged -= HealthStat_OnCurrentValueChanged;

            ShieldStat.OnCurrentValueChanged -= ShieldStat_OnCurrentValueChanged;
        }


        protected virtual void HealthStat_OnValueChanged(object sender, ValueChangedEventArgs args)
        {
            this.SetMaxValue(HealthStat.Value + ShieldStat.CurrentValue);
        }

        protected virtual void HealthStat_OnCurrentValueChanged(object sender, ValueChangedEventArgs args)
        {
            float life = HealthStat.CurrentValue + ShieldStat.CurrentValue;
            if (life > HealthStat.Value) this.SetMaxValue(life);
            else this.SetMaxValue(HealthStat.Value);

            HealthSlider.value = HealthStat.CurrentValue;
            ShieldSlider.value = life;
        }

        protected virtual void ShieldStat_OnCurrentValueChanged(object sender, ValueChangedEventArgs args)
        {
            float life = HealthStat.CurrentValue + ShieldStat.CurrentValue;
            if (life > HealthStat.Value) this.SetMaxValue(life);
            else this.SetMaxValue(HealthStat.Value);

            ShieldSlider.value = life;
        }

        protected virtual void SetMaxValue(float value)
        {
            HealthSlider.maxValue = value;
            ShieldSlider.maxValue = value;
        }
    }
}