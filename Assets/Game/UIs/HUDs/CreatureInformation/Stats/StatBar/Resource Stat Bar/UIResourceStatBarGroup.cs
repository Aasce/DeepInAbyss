using Asce.Game.Stats;
using Asce.Managers.UIs;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs.Stats
{
    public class UIResourceStatBarGroup<TUI, TStat> : UIObject 
        where TUI : IUIStatBar<TStat> 
        where TStat : Stat
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TUI _statBar;

        public Image Icon => _icon;
        public TUI StatBar => _statBar;

        public virtual void SetStat(TStat stat)
        {
            StatIconContainer container = UIStatManager.Instance.Data.GetStatIcon(stat.StatType);
            if (container == null) return;

            Sprite statIcon = container.Icon;
            Color statColor = container.Color;

            StatBar.SetStat(stat);
            if (Icon != null)
            {
                Icon.sprite = statIcon;
                Icon.color = statColor;
            }
        }
    }

    public class UIResourceStatBarGroup : UIResourceStatBarGroup<UIResourceStatBar, ResourceStat>
    {

    }
}