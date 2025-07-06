using Asce.Managers.Attributes;
using Asce.Managers.UIs;
using Asce.Managers.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Asce.Game.UIs
{
    public class UIWindow : UIObject, IPointerDownHandler
    {
        // Ref
        [SerializeField, Readonly] protected UIWindowsController _controler;
        [SerializeField, Readonly] protected UIWindowTitle _title;
        [SerializeField] protected Button _exitButton;
        [SerializeField] protected Button _helpButton;

        public UIWindowsController Controller
        {
            get => _controler;
            set => _controler = value;
        }
        public UIWindowTitle Title => _title;
        public Button ExitButton => _exitButton;
        public Button HelpButton => _helpButton;

        protected override void RefReset()
        {
            base.RefReset();
            if (this.LoadComponent(out _title)) 
            {
                Title.Window = this;
            }
        }
        protected virtual void Start()
        {
            if (ExitButton != null) ExitButton.onClick.AddListener(ExitButton_OnClick);
        }

        public virtual void Focus()
        {
            if (Controller != null) Controller.Focus(this);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            this.Focus();
        }
        protected virtual void ExitButton_OnClick()
        {
            this.Hide();
        }

    }
}
