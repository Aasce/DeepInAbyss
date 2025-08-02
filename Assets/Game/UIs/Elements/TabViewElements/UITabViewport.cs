using Asce.Managers.UIs;
using UnityEngine;

namespace Asce.Game.UIs
{
    public class UITabViewport : UIObject
    {
        [SerializeField] protected UITabGroup _tabGroup;

        public UITabGroup TabGroup
        {
            get => _tabGroup;
            set => _tabGroup = value;
        }
    }
}
