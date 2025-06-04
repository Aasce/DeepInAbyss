using Asce.Game.Combats;
using Asce.Managers;
using Asce.Managers.Pools;
using Asce.Managers.Utils;
using System.Collections;
using UnityEngine;

namespace Asce.Game.FloatingTexts
{
    /// <summary>
    ///     Manages the display and pooling of floating stat value popups such as damage, healing, or shield absorption.
    /// </summary>
    public class StatValuePopupManager : MonoBehaviorSingleton<StatValuePopupManager>
    {
        [Tooltip("Object pool used to reuse StatValuePopup instances efficiently.")]
        [SerializeField] protected Pool<StatValuePopup> _pools = new();

        [Header("Configs")]
        [Tooltip("Offset to apply to the popup's world position (e.g., to float above the source).")]
        [SerializeField] protected Vector3 _offset = Vector3.zero;

        [Header("Colors")]
        [Tooltip("Color used for True Damage popups.")]
        [SerializeField] protected Color _trueDamageColor;

        [Tooltip("Color used for Physical Damage popups.")]
        [SerializeField] protected Color _physicalDamageColor;

        [Tooltip("Color used for Magical Damage popups.")]
        [SerializeField] protected Color _magicalDamageColor;

        [Space]
        [Tooltip("Color used for healing popups.")]
        [SerializeField] protected Color _healColor;

        [Tooltip("Color used for absorbed shield damage popups.")]
        [SerializeField] protected Color _absorbedShieldColor;

        [Header("Size")]
        [Tooltip("Font size for single-digit values [0-9].")]
        [SerializeField] protected float _minSize = 30;

        [Tooltip("Font size for two-digit values [10-99].")]
        [SerializeField] protected float _normalSize = 40;

        [Tooltip("Font size for three-digit values [100-999].")]
        [SerializeField] protected float _largeSize = 55;

        [Tooltip("Font size for values with four or more digits [1000+].")]
        [SerializeField] protected float _extraSize = 70;

        public Pool<StatValuePopup> Pool => _pools;

        public Vector3 Offset
        {
            get => _offset;
            set => _offset = value;
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected virtual void Update()
        {
            // Automatically deactivate popups when their cooldown is complete
            Pool.DeactivateMatch((popup) =>
            {
                popup.DeactiveCooldown.Update(Time.deltaTime);
                return popup.DeactiveCooldown.IsComplete;
            });
        }

        /// <summary>
        ///     Creates a popup text for a damage event with color based on damage type.
        /// </summary>
        public virtual StatValuePopup CreateDamagePopupText(float damage, DamageType type, Vector2 position, float delay = 0f)
        {
            Color color = ColorByDamageType(type);
            float size = SizeByValue(damage);

            return CreateValuePopupDelay(Mathf.Round(damage).ToString(), color, size, position, delay);
        }

        /// <summary>
        ///     Creates a popup text for a healing event.
        /// </summary>
        public virtual StatValuePopup CreateHealPopupText(float heal, Vector2 position, float delay = 0f)
        {
            float size = SizeByValue(heal);
            return CreateValuePopupDelay($"+{Mathf.Round(heal)}", _healColor, size, position, delay);
        }

        /// <summary>
        ///     Creates a popup text for shield absorption (negative effect on shield).
        /// </summary>
        public virtual StatValuePopup CreateShieldAbsorptionPopupText(float absorbedShield, Vector2 position, float delay = 0f)
        {
            float size = SizeByValue(absorbedShield);
            return CreateValuePopupDelay($"-{Mathf.Round(absorbedShield)}", _absorbedShieldColor, size, position, delay);
        }

        /// <summary>
        ///     Creates a stat value popup immediately or with a delay.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="size">The size of the text.</param>
        /// <param name="position">World position for the popup.</param>
        /// <param name="delay">(Optionals) delay before showing the popup.</param>
        /// <returns>
        ///     Returns the created popup instance, or null if delayed.
        /// </returns>
        protected virtual StatValuePopup CreateValuePopupDelay(string text, Color color, float size, Vector2 position, float delay)
        {
            if (delay <= 0f)
            {
                return this.CreateValuePopup(text, color, size, position);
            }
            else
            {
                StartCoroutine(DelayedCreate(text, color, size, position, delay));
                return null;
            }
        }

        /// <summary>
        ///     Coroutine to delay the creation of a stat value popup.
        /// </summary>
        /// <param name="text"> Text to display. </param>
        /// <param name="color"> Text color. </param>
        /// <param name="size"> Text size. </param>
        /// <param name="position"> Position to display popup. </param>
        /// <param name="delay"> Delay in seconds. </param>
        protected virtual IEnumerator DelayedCreate(string text, Color color, float size, Vector2 position, float delay)
        {
            yield return new WaitForSeconds(delay);
            this.CreateValuePopup(text, color, size, position);
        }

        /// <summary>
        ///     Creates and displays a popup with the specified parameters.
        /// </summary>
        /// <param name="text"> Text to display. </param>
        /// <param name="color"> Color of the popup text. </param>
        /// <param name="size"> Font size of the text. </param>
        /// <param name="position"> World position for the popup. </param>
        /// <returns> The created popup instance. </returns>
        protected virtual StatValuePopup CreateValuePopup(string text, Color color, float size, Vector2 position)
        {
            StatValuePopup popup = Pool.Activate();
            popup.DeactiveCooldown.Reset();

            popup.Text = text ?? string.Empty;
            popup.Color = color;
            popup.Size = size;
            popup.transform.position = (Vector3)position + Offset;

            return popup;
        }

        /// <summary>
        ///     Determines the popup text color based on the <see cref="DamageType"/>.
        /// </summary>
        protected Color ColorByDamageType(DamageType type)
        {
            return type switch
            {
                DamageType.TrueDamage => _trueDamageColor,
                DamageType.Physical => _physicalDamageColor,
                DamageType.Magical => _magicalDamageColor,
                _ => Color.black,
            };
        }

        /// <summary>
        ///     Determines the popup text size based on the number of digits in the value.
        /// </summary>
        protected float SizeByValue(float value)
        {
            int digitCount = NumberUtils.GetIntegerDigitCount(value);
            return digitCount switch
            {
                1 => _minSize,
                2 => _normalSize,
                3 => _largeSize,
                >= 4 => _extraSize,
                _ => _normalSize,
            };
        }
    }
}
