using System;
using UnityEngine;

namespace Asce.Managers.UIs
{
    /// <summary>
    ///     Base class for UI components.
    /// </summary>
    public abstract class UIObject : MonoBehaviour, IUIObject
    {
        /// <summary>
        ///     Invoked when the UI object is hidden.
        /// </summary>
        public event Action<object> OnHide;

        /// <summary>
        ///     Invoked when the UI object is shown.
        /// </summary>
        public event Action<object> OnShow;

        /// <summary>
        ///     Gets the <see cref="UnityEngine.RectTransform"/> of the UI object.
        /// </summary>
        public RectTransform RectTransform => transform as RectTransform;

        /// <summary>
        ///     Gets whether the UI object is currently visible.
        /// </summary>
        public bool IsShow => gameObject.activeSelf;

        /// <summary>
        ///     Sets the visibility of the UI object.
        /// </summary>
        /// <param name="state"> If true, shows the object; otherwise, hides it. </param>
        public virtual void SetVisible(bool visible)
        {
            if (visible) this.Show();
            else this.Hide();
        }

        /// <summary>
        ///     Toggles the visibility of the UI object.
        /// </summary>
        public virtual void Toggle() => this.SetVisible(!this.IsShow);

        /// <summary>
        ///     Hides the UI object if it is currently shown.
        ///     Triggers the <see cref="OnHide"/> event.
        /// </summary>
        public virtual void Hide()
        {
            if (!this.IsShow) return;
            gameObject.SetActive(false);
            OnHide?.Invoke(this);
        }

        /// <summary>
        /// Shows the UI object if it is currently hidden.
        /// Triggers the <see cref="OnShow"/> event.
        /// </summary>
        public virtual void Show()
        {
            if (this.IsShow) return;
            gameObject.SetActive(true);
            OnShow?.Invoke(this);
        }
    }
}