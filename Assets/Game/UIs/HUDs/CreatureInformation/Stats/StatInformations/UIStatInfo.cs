using Asce.Game.Stats;
using Asce.Game.StatusEffects;
using Asce.Managers.Attributes;
using Asce.Managers.UIs;
using Asce.Managers.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Asce.Game.UIs.Stats
{
    public class UIStatInfo : UIObject, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler
    {
        [SerializeField, Readonly] protected Image _icon;
        [SerializeField, Readonly] protected TextMeshProUGUI _valueText;
        protected Stat _stat;

        public Image Icon => _icon;
        public TextMeshProUGUI ValueText => _valueText;
        public Stat Stat => _stat;


        protected override void RefReset()
        {
            base.RefReset();
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

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_stat == null) return;

            UIScreenCanvasManager.Instance.Tooltip.Caller = RectTransform;
            UIScreenCanvasManager.Instance.Tooltip.SetTooltip(
                size: new Vector2(200f, 67f),
                title: $"{_stat.StatType}",
                content: $"Value: {_stat.Value:#.#}\n{_stat.GetContent()}"
            );
            UIScreenCanvasManager.Instance.Tooltip.Show();
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            UIScreenCanvasManager.Instance.Tooltip.SetPositionFromScreen(eventData.position, new Vector2(4f, -4f));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            UIScreenCanvasManager.Instance.Tooltip.Hide();
        }
    }
}