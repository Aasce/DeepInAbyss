using Asce.Game.UIs.ContextMenus;
using Asce.Managers;
using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.UIs
{
    [RequireComponent(typeof(Canvas))]
    public class UIScreenCanvasManager : MonoBehaviourSingleton<UIScreenCanvasManager>
    {
        [SerializeField, Readonly] protected Camera _camera;
        [SerializeField, Readonly] protected Canvas _canvas;

        [SerializeField, Readonly] protected UIWindowsController _windowsController;
        [SerializeField, Readonly] protected UIContextMenusController _contextMenusController;
        [SerializeField, Readonly] protected UITooltip _tooltip;

        [Space]
        [SerializeField, Readonly] protected UIInteractableObjectController _interactableObjectController;

        public Camera Camera => _camera;
        public Canvas Canvas => _canvas;
        public UIWindowsController WindowsController => _windowsController;
        public UIContextMenusController ContextMenusController => _contextMenusController;
        public UITooltip Tooltip => _tooltip;

        public UIInteractableObjectController InteractableObjectController => _interactableObjectController;


        protected virtual void Reset()
        {
            this.LoadComponent(out _canvas, includeInactive: true);
            this.LoadComponent(out _windowsController, includeInactive: true);
            this.LoadComponent(out _contextMenusController, includeInactive: true);
            this.LoadComponent(out _tooltip, includeInactive: true);
            this.LoadComponent(out _interactableObjectController, includeInactive: true);
        }

        private void Start()
        {
            _camera = Players.Player.Instance.CameraController.Camera;
        }
    }
}
