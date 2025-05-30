using Asce.Game.Stats;
using Asce.Managers;
using Asce.Managers.UIs;
using Asce.Managers.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs
{
    public class UIResourceStatBar : UIObject
    {
        [SerializeField, HideInInspector] protected Slider _slider;
        protected ResourceStat _stat;

        public Slider Slider => _slider;
        public ResourceStat Stat => _stat;


        private void Reset()
        {
            this.LoadComponent(out  _slider);
        }

        public void SetStat(ResourceStat stat)
        {
            this.Unregister();

            this._stat = stat;
            this.Register();
        }

        protected virtual void Register()
        {
            if (Stat == null) return;

            Slider.maxValue = Stat.Value;
            Slider.value = Stat.CurrentValue;

            Stat.OnValueChanged += Stat_OnValueChanged;
            Stat.OnCurrentValueChanged += Stat_OnCurrentValueChanged;
        }

        protected virtual void Unregister()
        {
            if (Stat == null) return;
            Stat.OnValueChanged -= Stat_OnValueChanged;
            Stat.OnCurrentValueChanged -= Stat_OnCurrentValueChanged;
        }

        protected virtual void Stat_OnValueChanged(object sender, ValueChangedEventArgs args) 
        {
            Slider.maxValue = Stat.Value;
        }

        protected virtual void Stat_OnCurrentValueChanged(object sender, ValueChangedEventArgs args)
        {
            Slider.value = Stat.CurrentValue;
        }
    }
}