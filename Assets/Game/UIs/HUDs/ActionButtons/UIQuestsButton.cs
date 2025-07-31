using Asce.Managers.Attributes;
using Asce.Managers.UIs;
using Asce.Managers.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs
{
    [RequireComponent(typeof(Button))]
    public class UIQuestsButton : UIObject
    {
        /// <summary> The button component attached to this object. </summary>
        [SerializeField, Readonly] protected Button _button;

        /// <summary> Public accessor for the button component. </summary>
        public Button Button => _button;

        protected override void RefReset()
        {
            base.RefReset();
            this.LoadComponent(out _button);
        }

        protected virtual void Start()
        {
            if (Button != null)
            {
                Button.onClick.AddListener(Button_OnClick);
            }
        }

        protected virtual void Button_OnClick()
        {

        }
    }
}
