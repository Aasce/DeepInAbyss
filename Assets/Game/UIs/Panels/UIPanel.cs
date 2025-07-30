using Asce.Managers.Attributes;
using Asce.Managers.UIs;
using UnityEngine;

namespace Asce.Game.UIs.Panels
{
    public class UIPanel : UIObject
    {
        [SerializeField, Readonly] protected UIPanelsController _controller;

        public UIPanelsController Controller
        {
            get => _controller;
            set => _controller = value;
        }
    }
}
