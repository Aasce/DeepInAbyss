using Asce.Managers.UIs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Asce.Game.UIs.Panels
{
    public class UIPanelsController : UIObject
    {
        [SerializeField] protected List<UIPanel> _panels = new();
        protected ReadOnlyCollection<UIPanel> _readonlyPanels;
        protected Dictionary<Type, UIPanel> _panelByType = new();

        public ReadOnlyCollection<UIPanel> Panels => _readonlyPanels ??= _panels.AsReadOnly();


        protected virtual void Awake()
        {
            _panels.Clear();

            // Scans child objects for UIPanel components and registers them.
            foreach (Transform child in transform)
            {
                if (!child.TryGetComponent(out UIPanel panel)) continue;

                _panels.Add(panel);
                _panelByType[panel.GetType()] = panel;

                panel.Controller = this;

                // Avoid double subscriptions
                panel.OnHide -= Panel_OnHide;
                panel.OnShow -= Panel_OnShow;

                panel.OnHide += Panel_OnHide;
                panel.OnShow += Panel_OnShow;
            }
        }

        protected virtual void Start()
        {
            // Hides all registered panels when the controller starts.
            foreach (UIPanel panel in _panels)
            {
                if (panel == null) continue;
                panel.Hide();
            }
        }

        public T GetPanel<T>() where T : UIPanel
        {
            if (_panelByType.TryGetValue(typeof(T), out UIPanel panel))
                return panel as T;
            return null;
        }

        protected virtual void Panel_OnHide(object sender)
        {
            UIPanel panel = sender as UIPanel;

        }

        protected virtual void Panel_OnShow(object sender)
        {
            UIPanel panel = sender as UIPanel;
            panel.transform.SetAsLastSibling();
        }
    }
}
