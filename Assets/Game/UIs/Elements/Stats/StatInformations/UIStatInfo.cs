using Asce.Game.Stats;
using Asce.Managers.UIs;
using Asce.Managers.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs.Stats
{
    public class UIStatInfo : UIObject
    {
        [SerializeField, HideInInspector] protected Image _icon;
        [SerializeField, HideInInspector] protected TextMeshProUGUI _valueText;
        protected Stat _stat;

        public Image Icon => _icon;
        public TextMeshProUGUI ValueText => _valueText;
        public Stat Stat => _stat;


        protected virtual void Reset()
        {
            this.LoadComponent(out _icon);
            this.LoadComponent(out _valueText);
        }


        public virtual void SetStat(Stat stat, Sprite icon = null, Color color = default)
        {
            this.Unregister();

            this._stat = stat;
            if (icon != null) Icon.sprite = icon;
            if (color != default) Icon.color = color;

            this.Register();
        }

        protected virtual void Register()
        {
            if (Stat == null) return;

            ValueText.text = Stat.Value.ToString("0.#");
            Stat.OnValueChanged += Stat_OnValueChanged;
        }

        protected virtual void Unregister()
        {
            if (Stat == null) return;

            Stat.OnValueChanged -= Stat_OnValueChanged;
        }

        private void Stat_OnValueChanged(object sender, Managers.ValueChangedEventArgs args)
        {
            if (ValueText == null) return;
            ValueText.text = Stat.Value.ToString("0.#");
        }
    }
}