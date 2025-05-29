using Asce.Managers.Utils;
using TMPro;
using UnityEngine;

namespace Asce.Game.FloatingTexts
{
    /// <summary>
    ///     Controls the display of stat value floating text (e.g., damage, healing) in the game world.
    /// </summary>
    public class StatValuePopup : MonoBehaviour
    {
        [SerializeField, HideInInspector] protected TextMeshProUGUI _textMesh;

        [Tooltip("Cooldown duration before the popup is deactivated (in seconds).")]
        [SerializeField] protected Cooldown _deactiveCooldown = new(2f);


        /// <summary>
        ///     Gets the TextMeshProUGUI component.
        /// </summary>
        public TextMeshProUGUI TextMesh => _textMesh;

        /// <summary>
        ///     Gets the cooldown used to determine when to deactivate this popup.
        /// </summary>
        public Cooldown DeactiveCooldown => _deactiveCooldown;

        /// <summary>
        ///     Gets or sets the text displayed in the popup.
        /// </summary>
        public string Text
        {
            get => _textMesh.text;
            set => _textMesh.text = value;
        }

        /// <summary>
        ///     Gets or sets the color of the popup text.
        /// </summary>
        public Color Color
        {
            get => _textMesh.color;
            set => _textMesh.color = value;
        }

        /// <summary>
        ///     Gets or sets the font size of the popup text.
        /// </summary>
        public float Size
        {
            get => _textMesh.fontSize;
            set => _textMesh.fontSize = value;
        }


        protected virtual void Reset()
        {
            // Automatically assigns the required components in the editor if missing.
            this.LoadComponent(out _textMesh);
        }
    }
}
