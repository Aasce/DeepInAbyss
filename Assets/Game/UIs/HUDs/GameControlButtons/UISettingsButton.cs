using Asce.Managers.Attributes;
using Asce.Managers.UIs;
using Asce.Managers.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs
{
    public class UISettingsButton : UIObject
    {
        [SerializeField, Readonly] protected Button _button;

        protected override void RefReset()
        {
            base.RefReset();
            this.LoadComponent(out _button);
        }

        protected virtual void Start()
        {
            if (_button != null) _button.onClick.AddListener(Button_OnClick);
        }

        protected virtual void Button_OnClick()
        {
            UISettingsWindow window = UIScreenCanvasManager.Instance.WindowsController.GetWindow<UISettingsWindow>();
            if (window ==  null) return;
            window.Toggle();
        }
    }
}
