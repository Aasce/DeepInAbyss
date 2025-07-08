using Asce.Game.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs.ContextMenus
{
    public class UIItemContextMenu : UIContextMenu
    {
        [SerializeField] protected Image _icon;
        [SerializeField] protected Image _iconHolder;

        [SerializeField] protected TextMeshProUGUI _name;

        [SerializeField] protected Slider _quantitySlider;
        [SerializeField] protected TextMeshProUGUI _quantityText;

        protected Item _item;
        protected int _index;

        public int Index
        {
            get => _index;
            set => _index = value;
        }

        /// <summary>
        ///     if not show or _quantitySlider == null, return -1
        /// </summary>
        public int QuantityToSplit => _quantitySlider != null ? Mathf.RoundToInt(_quantitySlider.value) : -1;

        protected virtual void Start()
        {
            if (_quantitySlider != null) _quantitySlider.onValueChanged.AddListener(QuantitySlider_OnValueChanged);
        }

        public virtual void Set(Item item)
        {
            if (item.IsNull()) return;

            _item = item;
            this.SetIcon();
            this.SetName();
            this.SetQuantity();
            this.SetQuantityText();
        }

        public virtual void SetIcon()
        {
            if (_iconHolder != null)
            {

            }

            if (_icon != null)
            {
                _icon.sprite = _item.Information.Icon;
            }
        }

        public virtual void SetName()
        {
            if(_name == null) return;
            _name.text = _item.Information.Name;
        }

        public virtual void SetQuantity()
        {
            if (_quantitySlider == null) return;
            if (!_item.Information.HasProperty(ItemPropertyType.Stackable))
            {
                if (_quantityText != null) _quantityText.gameObject.SetActive(false);
                _quantitySlider.gameObject.SetActive(false);
                return;
            }

            if (_quantityText != null) _quantityText.gameObject.SetActive(true);
            _quantitySlider.gameObject.SetActive(true);

            int quantity = _item.GetQuantity();
            _quantitySlider.maxValue = quantity;
            _quantitySlider.value = Mathf.RoundToInt(Mathf.Ceil(quantity*0.5f));
        }

        public virtual void SetQuantityText()
        {
            if (_quantityText == null) return;
            _quantityText.text = $"{_quantitySlider.value}/{_quantitySlider.maxValue}";
        }

        protected virtual void QuantitySlider_OnValueChanged(float value)
        {
            this.SetQuantityText();
        }
    }
}