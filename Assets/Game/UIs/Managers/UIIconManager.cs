using Asce.Game.UIs.Stats;
using Asce.Managers;
using UnityEngine;

namespace Asce.Game.UIs
{
    public class UIIconManager : Singleton<UIIconManager>
    {
        public readonly string uiStatDataPath = "Stat Icon Data";

        [SerializeField] private SO_UIStatData _stats;


        public SO_UIStatData Stats => _stats != null ? _stats  : this.LoadUIStatData();


        private UIIconManager() 
        {

        }

        private SO_UIStatData LoadUIStatData()
        {
            _stats = Resources.Load<SO_UIStatData>(uiStatDataPath);
            if (_stats == null) Debug.LogError($"[{nameof(UIIconManager)}] {nameof(SO_UIStatData)} is null");
            return _stats;
        }
    }
}