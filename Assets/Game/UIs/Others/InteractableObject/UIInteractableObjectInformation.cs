using Asce.Game.Enviroments;
using Asce.Game.Players;
using Asce.Managers.UIs;
using Asce.Managers.Utils;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs
{
    public class UIInteractableObjectInformation : UIObject
    {
        [SerializeField] protected Image _icon;
        [SerializeField] protected Image _iconBackground;

        [SerializeField] protected TextMeshProUGUI _name;
        [SerializeField] protected Image _nameBackground;

        [SerializeField] protected TextMeshProUGUI _tip;

        protected IInteractableObject _interactableObject;


        public IInteractableObject InteractableObject => _interactableObject;


        public virtual void Set(IInteractableObject interactableObject)
        {
            if (interactableObject == _interactableObject) return;

            this.Unregister();
            _interactableObject = interactableObject;
            this.Register();
        }

        protected virtual void LateUpdate()
        {
            this.SetPosition();
        }

        protected virtual void SetPosition()
        {
            if (_interactableObject == null) return;

            // Get the world position of the interactable objectz
            Vector2 interactableObjectOffset = _interactableObject.Offset + Vector2.up * _interactableObject.InteractionRange * 0.5f;
            Vector2 interactableObjectWorldPosition = (Vector2)_interactableObject.gameObject.transform.position + interactableObjectOffset;

            // Convert world position to screen position
            Vector2 screenPosition = UIScreenCanvasManager.Instance.Camera.WorldToScreenPoint(interactableObjectWorldPosition);

            // Get canvas and its RectTransform
            Canvas canvas = UIScreenCanvasManager.Instance.Canvas;
            RectTransform canvasRect = canvas.transform as RectTransform;
            Camera canvasCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

            // Convert screen position to local position within the canvas
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, canvasCamera, out Vector2 localPos);

            // Set the final anchored position
            this.RectTransform.anchoredPosition = UICanvasUtils.ClampLocalAnchoredPosition(this.RectTransform, canvasRect, localPos);
        }


        protected virtual void Register()
        {
            if (_interactableObject == null) return;

            this.SetIcon();
            this.SetName();
            this.SetTip();

            this.SetToUnfocus();
            _interactableObject.OnFocus += InteractableObject_OnFocus;
            _interactableObject.OnUnfocus += InteractableObject_OnUnfocus;
        }

        protected virtual void Unregister()
        {
            if (_interactableObject == null) return;

            _interactableObject.OnFocus -= InteractableObject_OnFocus;
            _interactableObject.OnUnfocus -= InteractableObject_OnUnfocus;

        }

        protected virtual void SetIcon()
        {
            if (_icon == null) return;

        }

        protected virtual void SetName()
        {
            if (_name == null) return;
            _name.text = $"{_interactableObject.gameObject.name}";
        }

        protected virtual void SetTip()
        {
            if (_tip == null) return;
            _tip.text = $"Press [{Player.Instance.Settings.InteractionKey}] to Interaction";
            _tip.gameObject.SetActive(false);
        }

        protected virtual void InteractableObject_OnFocus(object sender)
        {
            if (_tip != null) _tip.gameObject.SetActive(true);
            if (_name != null) _name.DOColor(Color.green, 0.1f);

            if (_nameBackground != null) _nameBackground.DOColor(Color.white, 0.1f);
            if (_iconBackground != null) _iconBackground.DOColor(Color.white, 0.1f);
        }

        protected virtual void InteractableObject_OnUnfocus(object sender)
        {
            this.SetToUnfocus();
        }

        protected virtual void SetToUnfocus()
        {
            if (_tip != null) _tip.gameObject.SetActive(false);
            if (_name != null) _name.DOColor(Color.white, 0.1f);

            if (_nameBackground != null) _nameBackground.DOColor(Color.white * 0.8f, 0.1f);
            if (_iconBackground != null) _iconBackground.DOColor(Color.white * 0.8f, 0.1f);
        }
    }
}