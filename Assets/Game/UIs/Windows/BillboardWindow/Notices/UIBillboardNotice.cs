using Asce.Game.Enviroments;
using Asce.Managers.Attributes;
using Asce.Managers.UIs;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Asce.Game.UIs.Billboards
{
    public class UIBillboardNotice : UIObject, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] protected Outline _outline;
        [SerializeField] protected Image _paperBackground;
        [SerializeField] protected TextMeshProUGUI _title;
        [SerializeField] protected TextMeshProUGUI _content;

        [Space]
        [SerializeField, Readonly] protected Notice _notice;


        public Notice Notice => _notice;


        public virtual void SetNotice(Notice notice)
        {
            if (_notice == notice) return;

            this.Unregister();
            _notice = notice;
            this.Register();
        }
        protected virtual void Register()
        {
            if (_notice == null) return;
            this.SetTitle();
            this.SetContent();
        }

        protected virtual void Unregister()
        {
            if (_notice == null) return;

        }

        protected virtual void SetTitle()
        {
            if (_title == null) return;
            _title.text = _notice.Name;
        }

        protected virtual void SetContent()
        {
            if (_content == null) return;
            _content.text = _notice.Description;
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            transform.DOScale(Vector3.one * 1.05f, 0.1f);
            if (_outline != null) _outline.enabled = true;
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            transform.DOScale(Vector3.one, 0.1f);
            if (_outline != null) _outline.enabled = false;
        }
    }
}
