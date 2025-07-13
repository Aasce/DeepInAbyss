using Asce.Game.StatusEffects;
using Asce.Managers.UIs;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Asce.Game.UIs.StatusEffects
{
    public class UIStatusEffect : UIObject, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler
    {
        [SerializeField] protected Image _icon;
        [SerializeField] protected Image _durationFilter;
        [SerializeField] protected TextMeshProUGUI _stackText;

        protected StatusEffect _statusEffect;

        public Image Icon => _icon;
        public Image DurationFilter => _durationFilter;
        public TextMeshProUGUI StackText => _stackText;

        public StatusEffect StatusEffect => _statusEffect;


        public override void Hide()
        {
            base.Hide();
            if (UIScreenCanvasManager.Instance.Tooltip.Caller == RectTransform)
                UIScreenCanvasManager.Instance.Tooltip.Hide();
        }

        public virtual void SetStatusEffect(StatusEffect statusEffect)
        {
            this.Unregister();
            _statusEffect = statusEffect;
            this.Register();
        }

        protected virtual void Register()
        {
            if (StatusEffect.IsNull()) return;

            // Icon
            if (Icon != null)
            {
                Icon.sprite = StatusEffect.Information.Icon;
            }

            // Duration Filter
            if (DurationFilter != null)
            {
                StatusEffect.Duration.OnBaseTimeChanged += StatusEffecDuration_OnCurrentTimeChanged;
                StatusEffect.Duration.OnCurrentTimeChanged += StatusEffecDuration_OnCurrentTimeChanged;
            }

            // Stack Text
            if (StackText != null)
            {
                if (StatusEffect is StackStatusEffect stackStatusEffect)
                {
                    stackStatusEffect.OnCurrentStackChanged += StackStatusEffect_OnCurrentStackChanged;
                    if (stackStatusEffect.CurrentStack > 1)
                    {
                        StackText.gameObject.SetActive(true);
                        _stackText.text = stackStatusEffect.CurrentStack.ToString();
                    }
                    else StackText.gameObject.SetActive(false);
                }
                else StackText.gameObject.SetActive(false);
            }
        }

        protected virtual void Unregister()
        {
            if (StatusEffect.IsNull()) return;

            // Duration Filter
            if (DurationFilter != null)
            {
                StatusEffect.Duration.OnBaseTimeChanged -= StatusEffecDuration_OnCurrentTimeChanged;
                StatusEffect.Duration.OnCurrentTimeChanged -= StatusEffecDuration_OnCurrentTimeChanged;
            }

            // Stack Text
            if (StackText != null)
            {
                if (StatusEffect is StackStatusEffect stackStatusEffect)
                {
                    stackStatusEffect.OnCurrentStackChanged -= StackStatusEffect_OnCurrentStackChanged;
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_statusEffect == null) return;

            string stackString = string.Empty;
            if (_statusEffect is StackStatusEffect stackStatusEffect)
            {
                stackString = $"\nStack: {stackStatusEffect.CurrentStack}";
            }

            UIScreenCanvasManager.Instance.Tooltip.Caller = RectTransform;;
            UIScreenCanvasManager.Instance.Tooltip.SetTooltip(
                title: _statusEffect.Information.Name,
                content: _statusEffect.Information.Description,
                footer: $"Level: {_statusEffect.Level}\nStrength: {_statusEffect.Strength:0.##}{stackString}"
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

        protected virtual void StatusEffecDuration_OnCurrentTimeChanged(object sender, float value)
        {
            DurationFilter.fillAmount = _statusEffect.Duration.Ratio;
        }

        protected virtual void StackStatusEffect_OnCurrentStackChanged(object sender, int stack)
        {
            if (StatusEffect is not StackStatusEffect stackStatusEffect)
            {
                StackText.gameObject.SetActive(false);
                return;
            }

            if (stackStatusEffect.CurrentStack > 1)
            {
                StackText.gameObject.SetActive(true);
                _stackText.text = stackStatusEffect.CurrentStack.ToString();
            }
            else StackText.gameObject.SetActive(false);
        }
    }
}
