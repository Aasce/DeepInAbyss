using Asce.Managers;
using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.UIs
{
    [RequireComponent(typeof(Canvas))]
    public class UIScreenCanvasManager : MonoBehaviourSingleton<UIScreenCanvasManager>
    {
        [SerializeField, Readonly] protected Canvas _canvas;
        [SerializeField, Readonly] protected UIWindowsController _windowsController;

        public Canvas Canvas => _canvas;
        public UIWindowsController WindowsController => _windowsController;


        protected virtual void Reset()
        {
            this.LoadComponent(out _canvas);
            this.LoadComponent(out _windowsController);
        }
    }
}
