using Asce.Managers.UIs;
using Asce.Managers.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Managers.UIs
{
    [RequireComponent(typeof(Button))]
    public class TextButton : UIObject
    {
        [SerializeField] protected Button _button;
        [SerializeField] protected TextMeshProUGUI _text;


        public Button Button => _button;
        public TextMeshProUGUI Text => _text;

        protected override void RefReset()
        {
            base.RefReset();
            this.LoadComponent(out _button);
            this.LoadComponent(out _text);
        }
    }
}