using Asce.Managers.Attributes;
using Asce.Managers.UIs;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Asce.Game.UIs
{
    public class UIWindowTitle : UIObject, IPointerDownHandler, IDragHandler
    {
        [SerializeField, Readonly] protected UIWindow _window;
        [SerializeField] protected TextMeshProUGUI _textMesh;

        private Vector2 _offset;

        public UIWindow Window
        {
            get => _window;
            set => _window = value;
        }
        public TextMeshProUGUI TextMesh => _textMesh;


        public virtual void SetText(string text)
        {
            if (_textMesh == null) return;
            _textMesh.text = text;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (Window == null) return;
            Window.Focus();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                Window.RectTransform, eventData.position, eventData.pressEventCamera, out _offset);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (Window == null) return;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                Window.RectTransform.parent as RectTransform, eventData.position, eventData.pressEventCamera, out Vector2 pointerPos))
            {
                Window.RectTransform.localPosition = pointerPos - _offset;
            }
        }
    }
}
