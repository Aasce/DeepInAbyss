using Asce.Game.Stats;
using Asce.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Asce.Game.UIs.Stats
{
    public class UIHealthBar : UITimeBasedResourceStatBar, IUIStatBar<TimeBasedResourceStat>, IUIStatBarHasText, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] protected Slider _shieldSlider;

        protected ResourceStat _shieldStat;

        public Slider ShieldSlider => _shieldSlider;

        public ResourceStat ShieldStat
        {
            get => _shieldStat;
            protected set
            {
                _shieldStat = value;
            }
        }

        protected float ShieldValue => ShieldStat == null ? 0f : ShieldStat.CurrentValue;
        public override float TotalResource => base.TotalResource + ShieldValue;

        public void SetStat(TimeBasedResourceStat healthStat, ResourceStat shieldStat)
        {
            base.SetStat(healthStat);
            this.SetShield(shieldStat);
        }

        public virtual void SetShield(ResourceStat shieldStat)
        {
            this.UnregisterShield();
            ShieldStat = shieldStat;
            this.RegisterShield();
        }

        protected virtual void RegisterShield()
        {
            if (Stat != null && ShieldStat != null) ShieldStat.OnCurrentValueChanged += ShieldStat_OnCurrentValueChanged;
        }
        protected virtual void UnregisterShield()
        {
            if (ShieldStat != null) ShieldStat.OnCurrentValueChanged -= ShieldStat_OnCurrentValueChanged;
        }

        protected override void Stat_OnCurrentValueChanged(object sender, ValueChangedEventArgs args)
        {
            base.Stat_OnCurrentValueChanged(sender, args);
            ShieldSlider.value = TotalResource;
        }

        protected virtual void ShieldStat_OnCurrentValueChanged(object sender, ValueChangedEventArgs args)
        {
            this.SetMaxValue(Mathf.Max(TotalResource, Stat.Value));
            ShieldSlider.value = TotalResource;
            this.TriggerText();
        }

        protected override void SetMaxValue(float value)
        {
            base.SetMaxValue(value);
            ShieldSlider.maxValue = value;
        }

        protected override void ResetStatBar()
        {
            base.ResetStatBar();
            ShieldSlider.value = 0f;
        }

        protected override void SyncStatbar()
        {
            base.SyncStatbar();
            ShieldSlider.value = TotalResource;
        }
    }
}