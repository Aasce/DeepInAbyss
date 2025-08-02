using Asce.Managers.Attributes;
using Asce.Managers.UIs;
using Asce.Managers.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs
{
    [RequireComponent(typeof(Button))]
    public class UITabButton : UIObject
    {
        [SerializeField, Readonly] protected Button _button;
        [SerializeField] protected UITabGroup _tabGroup;

        [Space]
        [SerializeField] protected int _index = -1;

        public Button Button => _button;

        public UITabGroup TabGroup
        {
            get => _tabGroup; 
            set => _tabGroup = value;
        }

        public int Index
        {
            get => _index;
            set => _index = value;
        }

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
            if (_tabGroup != null) _tabGroup.Select(this);
        }
    }
}
