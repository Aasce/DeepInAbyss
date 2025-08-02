using Asce.Managers.UIs;
using UnityEngine;

namespace Asce.Game.UIs
{
    [System.Serializable]
    public class TabPair
    {
        [SerializeField] protected UITabButton _tab;
        [SerializeField] protected UITabViewport _viewPort;

        public UITabButton Tab => _tab;
        public UITabViewport Viewport => _viewPort;


        public TabPair() { }
        public TabPair(UITabButton button, UITabViewport viewPort)
        {
            _tab = button;
            _viewPort = viewPort;
        }
    }
}
