using Asce.Managers.UIs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs
{
    public class UITooltip : UIObject
    {
        [SerializeField] protected LayoutElement _layoutElement;

        [Space]
        [SerializeField] protected TextMeshProUGUI _title;
        [SerializeField] protected TextMeshProUGUI _content;
        [SerializeField] protected TextMeshProUGUI _footer;

        [Space]
        [SerializeField] protected Image _titleAndContentDivider;
        [SerializeField] protected Image _footerAndContentDivider;

        [Header("Configs")]
        [SerializeField, Min(0f)] protected float _minWidth = 150f;
        [SerializeField, Min(0f)] protected float _maxWidth = 500f;


        protected RectTransform _caller;

        public TextMeshProUGUI Title => _title;
        public TextMeshProUGUI Content => _content;
        public TextMeshProUGUI Footer => _footer;

        public float MinWidth
        {
            get => _minWidth;
            set => _minWidth = value;
        }

        public float MaxWidth
        {
            get => _maxWidth;
            set => _maxWidth = value;
        }

        public RectTransform Caller
        {
            get => _caller;
            set => _caller = value;
        }


        protected virtual void Start()
        {
            this.Hide();
        }

        public virtual void SetTooltip(Vector2 size, string title, string content, string footer = null)
        {
            SetSize(size);
            SetTitle(title);
            SetContent(content);
            SetFooter(footer);
        }

        public virtual void SetTooltip(string title, string content, string footer = null)
        {
            SetTitle(title);
            SetContent(content);
            SetFooter(footer);
            AutoSize();
        }

        public virtual void AutoSize()
        {
            float titleWidth = _title != null && _title.gameObject.activeSelf
                ? _title.GetPreferredValues(_title.text, Mathf.Infinity, Mathf.Infinity).x
                : 0f;

            float contentWidth = _content != null && _content.gameObject.activeSelf
                ? _content.GetPreferredValues(_content.text, Mathf.Infinity, Mathf.Infinity).x
                : 0f;

            float footerWidth = _footer != null && _footer.gameObject.activeSelf
                ? _footer.GetPreferredValues(_footer.text, Mathf.Infinity, Mathf.Infinity).x
                : 0f;

            float maxPreferredWidth = Mathf.Max(titleWidth, contentWidth, footerWidth);
            float finalWidth = Mathf.Clamp(maxPreferredWidth, MinWidth, MaxWidth);

            _layoutElement.preferredWidth = finalWidth;
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.RectTransform);
        }

        public virtual void SetPosition(Vector2 position)
        {
            this.RectTransform.anchoredPosition = position;
        }

        public virtual void SetPositionFromScreen(Vector2 screenPosition, Vector2? offset = null)
        {
            Canvas canvas = UIScreenCanvasManager.Instance.Canvas;
            RectTransform canvasRect = canvas.transform as RectTransform;
            Camera camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPosition,
                camera,
                out Vector2 localPoint
            );

            if (offset.HasValue) localPoint += offset.Value;

            this.RectTransform.anchoredPosition = localPoint;
        }

        public virtual void SetSize(Vector2 size)
        {
            this.RectTransform.sizeDelta = size;
        }

        public virtual void SetTitle(string text)
        {
            SetTextAndVisibility(_title, text);
            UpdateDividers();
        }

        public virtual void SetContent(string text)
        {
            SetTextAndVisibility(_content, text);
            UpdateDividers();
        }

        public virtual void SetContentFormat(string text, params object[] args)
        {
            if (_content == null) return;

            bool hasContent = !string.IsNullOrEmpty(text);
            _content.gameObject.SetActive(hasContent);
            _content.text = hasContent ? string.Format(text, args) : string.Empty;
            UpdateDividers();
        }

        public virtual void SetFooter(string text)
        {
            SetTextAndVisibility(_footer, text);
            UpdateDividers();
        }

        public virtual void Clear()
        {
            SetTitle(null);
            SetContent(null);
            SetFooter(null);
        }

        protected virtual void SetTextAndVisibility(TextMeshProUGUI label, string text)
        {
            if (label == null) return;

            bool hasText = !string.IsNullOrEmpty(text);
            label.gameObject.SetActive(hasText);
            label.text = text ?? string.Empty;
        }

        protected virtual void UpdateDividers()
        {
            bool hasTitle = _title != null && _title.gameObject.activeSelf;
            bool hasContent = _content != null && _content.gameObject.activeSelf;
            bool hasFooter = _footer != null && _footer.gameObject.activeSelf;

            int visibleCount = (hasTitle ? 1 : 0) + (hasContent ? 1 : 0) + (hasFooter ? 1 : 0);
            if (visibleCount <= 1)
            {
                SetDividerActive(_titleAndContentDivider, false);
                SetDividerActive(_footerAndContentDivider, false);
                return;
            }

            if (!hasContent)
            {
                SetDividerActive(_titleAndContentDivider, hasTitle && hasFooter);
                SetDividerActive(_footerAndContentDivider, false);
                return;
            }

            SetDividerActive(_titleAndContentDivider, hasTitle);
            SetDividerActive(_footerAndContentDivider, hasFooter);
        }

        protected void SetDividerActive(Image divider, bool active)
        {
            if (divider != null)
                divider.gameObject.SetActive(active);
        }
    }
}
