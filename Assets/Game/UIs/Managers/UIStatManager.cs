using Asce.Game.UIs.Stats;
using Asce.Managers;
using UnityEngine;

namespace Asce.Game.UIs
{
    public class UIStatManager : Singleton<UIStatManager>
    {
        public readonly string uiStatDataPath = "Data/Stat Data";

        [SerializeField] private SO_UIStatData _data;


        public SO_UIStatData Data => _data != null ? _data  : this.LoadUIStatData();


        private UIStatManager() 
        {

        }

        private SO_UIStatData LoadUIStatData()
        {
            _data = Resources.Load<SO_UIStatData>(uiStatDataPath);
            if (_data == null) Debug.LogError($"[{nameof(UIStatManager)}] {nameof(SO_UIStatData)} is null");
            return _data;
        }
    }
}