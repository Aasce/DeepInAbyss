using Asce.Game.UIs.Inventories;
using Asce.Managers.Attributes;
using Asce.Managers.UIs;
using Asce.Managers.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs
{
    /// <summary>
    ///     UI component that controls the inventory button in the HUD.
    ///     When clicked, it toggles the visibility of the inventory window.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class UIInventoryButton : UIObject
    {
        /// <summary>
        ///     The button component attached to this object.
        /// </summary>
        [SerializeField, Readonly] protected Button _button;

        /// <summary>
        ///     Cached reference to the inventory window.
        /// </summary>
        protected UIInventoryWindow _inventoryWindow;

        /// <summary>
        ///     Public accessor for the button component.
        /// </summary>
        public Button Button => _button;

        /// <summary>
        ///     Automatically finds and assigns the Button component.
        ///     Called by Unity or the editor when resetting references.
        /// </summary>
        protected override void RefReset()
        {
            base.RefReset();
            this.LoadComponent(out _button); // Custom helper for GetComponent
        }

        /// <summary>
        ///     Subscribes the click listener to the button on startup.
        /// </summary>
        protected virtual void Start()
        {
            if (Button != null)
            {
                Button.onClick.AddListener(Button_OnClick);
            }
        }

        /// <summary>
        ///     Called when the button is clicked. Toggles the inventory window.
        /// </summary>
        protected virtual void Button_OnClick()
        {
            // Lazily fetch the inventory window reference
            if (_inventoryWindow == null)
                _inventoryWindow = UIScreenCanvasManager.Instance.WindowsController.GetWindow<UIInventoryWindow>();

            if (_inventoryWindow == null) return;

            _inventoryWindow.Toggle();
        }
    }
}