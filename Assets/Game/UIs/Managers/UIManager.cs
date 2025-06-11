using Asce.Managers;
using UnityEngine;

namespace Asce.Game.UIs
{
    public class UIManager : Singleton<UIManager>
    {
        public readonly string uiTextDataPath = "Data/Text Data";

        [SerializeField] private SO_UITextData _textData;


        public SO_UITextData TextData => _textData != null ? _textData : this.LoadUIStatData();


        private UIManager()
        {

        }

        private SO_UITextData LoadUIStatData()
        {
            _textData = Resources.Load<SO_UITextData>(uiTextDataPath);
            if (_textData == null) Debug.LogError($"[{nameof(UIManager)}] {nameof(SO_UITextData)} is null");
            return _textData;
        }
    }
}