using Asce.Managers.UIs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Asce.Game.UIs.ContextMenus
{
    public class UIContextMenusController : UIObject
    {
        [SerializeField] protected List<UIContextMenu> _contextMenus = new();

        protected Dictionary<Type, UIContextMenu> _menuByType = new();
        protected ReadOnlyCollection<UIContextMenu> _readonlyMenus;
        public ReadOnlyCollection<UIContextMenu> ContextMenus => _readonlyMenus ??= _contextMenus.AsReadOnly();

        protected virtual void Awake()
        {
            _contextMenus.Clear();
            foreach (Transform child in transform)
            {
                if (!child.TryGetComponent(out UIContextMenu menu)) continue;

                _contextMenus.Add(menu);
                _menuByType[menu.GetType()] = menu;

            }
        }


        protected virtual void Start()
        {
            // Hides all registered menu when the controller starts.
            foreach (UIContextMenu menu in _contextMenus)
            {
                if (menu == null) continue;
                menu.Hide();
            }
        }

        public T GetMenu<T>() where T : UIContextMenu
        {
            if (_menuByType.TryGetValue(typeof(T), out UIContextMenu menu))
                return menu as T;
            return null;
        }
    }
}
