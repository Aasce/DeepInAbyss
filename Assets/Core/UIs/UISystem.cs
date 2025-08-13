using Asce.Managers.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Managers.UIs
{
    public static class UISystem
    {
        private static UIAlert _alert;
        private static Canvas _canvas;

        public static UIAlert GetAlert()
        {
            if (_alert == null) _alert = Resources.Load<UIAlert>("UIs/Alert");
            if (_alert == null)
            {
                return null;
            }
            List<Canvas> canvases = ComponentUtils.FindAllComponentsInScene<Canvas>();

            if (_canvas == null)
                foreach (Canvas canvas in canvases)
                {
                    if (canvas == null) continue;
                    if (canvas.renderMode != RenderMode.ScreenSpaceOverlay) continue;
                    _canvas = canvas;
                    break;
                }
            if (_canvas == null) return null;

            UIAlert newAlert = GameObject.Instantiate(_alert, _canvas.transform, false);
            newAlert.RectTransform.SetAsLastSibling();
            return newAlert;
        }
    }
}
