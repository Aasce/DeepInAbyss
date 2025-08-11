using Asce.Managers.UIs;
using Asce.Managers.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs
{
    [RequireComponent(typeof(Button))]
    public class ButtonSFX : UIObject
    {
        [SerializeField] protected Button _button;
        [SerializeField] protected string _soundName;

        public Button Button => _button;
        public string SoundName => _soundName;


        protected override void RefReset()
        {
            base.RefReset();
            this.LoadComponent(out _button);
        }

        private void Start()
        {
            if (_button != null) _button.onClick.AddListener(Button_OnClick);
        }

        protected virtual void Button_OnClick()
        {
            if (string.IsNullOrEmpty(_soundName)) return;
            Sounds.AudioManager.Instance.PlaySFX(_soundName);
        }
    }
}
