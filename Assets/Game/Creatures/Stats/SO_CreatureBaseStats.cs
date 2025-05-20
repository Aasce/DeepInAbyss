using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Entities
{
    [CreateAssetMenu(menuName = "Asce/Entities/Creature Base Stats", fileName = "Creature Base Stats")]
    public class SO_CreatureBaseStats : ScriptableObject, IBaseStatsData
    {
        [Header("Survival")]
        [SerializeField, Min(0f)] protected float _maxHealth;

        [Tooltip("Amount of health regenerated per second")]
        [SerializeField, Min(0f)] protected float _healthRegen;

        [Space]
        [SerializeField, Min(0f)] protected float _maxStamina;

        [Tooltip("Amount of stamina regenerated per second")]
        [SerializeField, Min(0f)] protected float _staminaRegen;

        [Space]
        [SerializeField, Min(0f)] protected float _maxHunger;

        [Tooltip("Rate at which hunger decreases every 10 seconds")]
        [SerializeField, Min(0f)] protected float _hungry;

        [Space]
        [SerializeField, Min(0f)] protected float _maxThirst;

        [Tooltip("Rate at which thirst decreases every 10 seconds")]
        [SerializeField, Min(0f)] protected float _thirsty;

        [Space]
        [SerializeField, Min(0f)] protected float _maxBreath;

        [Tooltip("Rate of breath recovery per second when the player can breathe")]
        [SerializeField, Min(0f)] protected float _breathe;

        [Header("Combat")]
        [SerializeField] protected float _strength;
        [SerializeField] protected float _armor;
        [SerializeField] protected float _resistance;

        [Header("Utilities")]
        [SerializeField, Min(0f)] protected float _speed;
        [SerializeField, Min(0f)] protected float _viewRadius;


        public float MaxHealth => _maxHealth;
        public float HealthRegen => _healthRegen;

        public float MaxStamina => _maxStamina;
        public float StaminaRegen => _staminaRegen;

        public float MaxHunger => _maxHunger;
        public float Hungry => _hungry;

        public float MaxThirst => _maxThirst;
        public float Thirsty => _thirsty;

        public float MaxBreath => _maxBreath;
        public float Breathe => _breathe;

        public float Strength => _strength;
        public float Armor => _armor;
        public float Resistance => _resistance;

        public float Speed => _speed;
        public float ViewRadius => _viewRadius;
    }
}
