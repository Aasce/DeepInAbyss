using Asce.Managers.Attributes;
using Asce.Managers.UIs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Asce.Game.UIs
{
    /// <summary>
    ///     Manages all active UIWindow instances in the UI Canvas.
    ///     Responsible for registering windows, handling focus, and showing/hiding behavior.
    /// </summary>
    public class UIWindowsController : UIObject
    {
        [Tooltip("All UIWindow instances found under this controller's transform.")]
        [SerializeField, Readonly] protected List<UIWindow> _windows = new();

        [Tooltip("Currently focused (top-most) window.")]
        [SerializeField] protected UIWindow _focusWindow;

        /// <summary> Lookup cache for quick access to windows by type. </summary>
        protected Dictionary<Type, UIWindow> _windowByType = new();

        /// <summary> Cached read-only reference to window list. </summary>
        protected ReadOnlyCollection<UIWindow> _readonlyWindows;

        /// <summary> All registered UIWindow instances. </summary>
        public ReadOnlyCollection<UIWindow> Windows => _readonlyWindows ??= _windows.AsReadOnly();

        /// <summary> The currently focused (front-most) window. </summary>
        public UIWindow FocusWindow
        {
            get => _focusWindow;
            protected set => _focusWindow = value;
        }

        protected virtual void Awake()
        {
            _windows.Clear();

            // Scans child objects for UIWindow components and registers them.
            foreach (Transform child in transform)
            {
                if (!child.TryGetComponent(out UIWindow window)) continue;

                _windows.Add(window);
                _windowByType[window.GetType()] = window;

                window.Controller = this;

                // Avoid double subscriptions
                window.OnHide -= Window_OnHide;
                window.OnShow -= Window_OnShow;

                window.OnHide += Window_OnHide;
                window.OnShow += Window_OnShow;
            }
        }

        protected virtual void Start()
        {
            // Hides all registered windows when the controller starts.
            foreach (UIWindow window in _windows)
            {
                if (window == null) continue;
                window.Hide();
            }
        }

        /// <summary>
        ///     Checks whether the given window is currently focused.
        /// </summary>
        /// <param name="window"> The window to check. </param>
        /// <returns> True if it is the focused window. </returns>
        public virtual bool IsFocus(UIWindow window) => FocusWindow != null && FocusWindow == window;

        /// <summary>
        ///     Brings the given window to front and sets it as the focused window.
        /// </summary>
        /// <param name="window"> The window to focus. </param>
        public virtual void Focus(UIWindow window)
        {
            if (window == null) return;
            if (!window.IsShow) return;
            if (!_windows.Contains(window)) return;

            FocusWindow = window;
            window.transform.SetAsLastSibling(); // Bring to front
        }

        /// <summary>
        ///     Unfocus the current focused window by hiding it.
        ///     Focus will automatically shift to the next active window via Window_OnHide.
        /// </summary>
        /// <returns>
        ///     True if a focused window was unfocused, false if there was no focused window.
        /// </returns>
        public virtual bool Unfocus()
        {
            if (FocusWindow == null)
                return false;

            FocusWindow.Hide();
            return true;
        }

        /// <summary>
        ///     Gets a window of a specific type, if registered.
        /// </summary>
        /// <typeparam name="T"> Type of <see cref="UIWindow"/> to retrieve. </typeparam>
        /// <returns> The found window instance, or null. </returns>
        public T GetWindow<T>() where T : UIWindow
        {
            if (_windowByType.TryGetValue(typeof(T), out UIWindow window))
                return window as T;
            return null;
        }

        /// <summary>
        ///     Gets a window by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UIWindow GetWindowByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            foreach (UIWindow window in _windows)
            {
                if (window == null) continue;
                if (window.name == name) return window;
            }
            return null;
        }

        /// <summary>
        ///     Called when a window is hidden. Automatically shifts focus to the next top-most window.
        /// </summary>
        /// <param name="sender"> The window that was hidden. </param>
        protected virtual void Window_OnHide(object sender)
        {
            UIWindow closedWindow = sender as UIWindow;
            if (!this.IsFocus(closedWindow)) return;
            
            UIWindow topMostWindow = null;
            int highestIndex = -1;
            foreach (UIWindow currentWindow in _windows)
            {
                if (currentWindow == null || !currentWindow.IsShow || currentWindow == closedWindow)
                    continue;

                int windowIndex = currentWindow.transform.GetSiblingIndex();
                if (windowIndex > highestIndex)
                {
                    highestIndex = windowIndex;
                    topMostWindow = currentWindow;
                }
            }

            if (topMostWindow != null) this.Focus(topMostWindow);
            else FocusWindow = null;
        }

        /// <summary>
        ///     Called when a window is shown. Automatically sets it as the focused window.
        /// </summary>
        /// <param name="sender"> The window that was shown. </param>
        protected virtual void Window_OnShow(object sender)
        {
            UIWindow window = sender as UIWindow;
            this.Focus(window);
        }
    }
}
