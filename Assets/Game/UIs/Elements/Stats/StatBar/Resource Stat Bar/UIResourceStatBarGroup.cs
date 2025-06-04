using Asce.Managers.UIs;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs.Stats
{
    public class UIResourceStatBarGroup<T> : UIObject where T : UIResourceStatBar
    {
        [SerializeField] private Image _icon;
        [SerializeField] private T _statBar;

        public Image Icon => _icon;
        public T StatBar => _statBar;
    }

    public class UIResourceStatBarGroup : UIResourceStatBarGroup<UIResourceStatBar>
    {

    }
}