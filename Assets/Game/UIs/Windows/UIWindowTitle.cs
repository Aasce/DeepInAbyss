using Asce.Managers.Attributes;
using Asce.Managers.UIs;
using Asce.Managers.Utils;
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
            if (Window == null || UIScreenCanvasManager.Instance == null) return;

            RectTransform canvasRect = UIScreenCanvasManager.Instance.Canvas.transform as RectTransform;
            if (canvasRect == null) return;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, eventData.position, eventData.pressEventCamera, out Vector2 pointerPos))
            {
                Vector2 targetLocalPos = pointerPos - _offset;

                // Clamp the window position within the canvas
                Vector2 clampedPos = UICanvasUtils.ClampLocalAnchoredPosition(Window.RectTransform, canvasRect, targetLocalPos);
                Window.RectTransform.localPosition = clampedPos;
            }
        }

    }
}
