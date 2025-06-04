using Asce.Game.Stats;
using Asce.Managers.Pools;
using Asce.Managers.UIs;
using UnityEngine;

namespace Asce.Game.UIs.Stats
{
    public class UIStatsInfoController : UIObject
    {
        [SerializeField] protected Pool<UIStatInfo> _uiPool = new();

        public virtual void AddStat(Stat stat)
        {
            if (stat == null) return;

            UIStatInfo foundUIStat = _uiPool.Activities.Find(findStat => findStat.Stat == stat);
            if (foundUIStat != null) return;

            UIStatInfo newUIStat = _uiPool.Activate();

            StatDataContainer container = UIIconManager.Instance.Stats.GetStat(stat.StatType);
            if (container == null) return;

            Sprite statIcon = container.Icon;
            Color statColor = container.Color;

            newUIStat.SetStat(stat, statIcon, statColor);
        }
    }
}