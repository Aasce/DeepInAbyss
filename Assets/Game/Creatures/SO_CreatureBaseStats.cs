using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Entities
{
    [CreateAssetMenu(menuName = "Asce/Entities/Creature Base Stats", fileName = "Creature Base Stats")]
    public class SO_CreatureBaseStats : ScriptableObject, IBaseStatsData
    {
        [Header("Survival")]
        [SerializeField, Min(0f)] protected float _maxHealth;
        [SerializeField, Min(0f)] protected float _maxStamina;
        [SerializeField, Min(0f)] protected float _maxHunger;
        [SerializeField, Min(0f)] protected float _maxThirsty;

        [Header("Combat")]
        [SerializeField] protected float _strength;
        [SerializeField] protected float _armor;
        [SerializeField] protected float _resistance;

        [Header("Utilities")]
        [SerializeField, Min(0f)] protected float _speed;


        public float MaxHealth => _maxHealth;
        public float MaxStamina => _maxStamina;
        public float MaxHunger => _maxHunger;
        public float MaxThirsty => _maxThirsty;

        public float Strength => _strength;
        public float Armor => _armor;
        public float Resistance => _resistance;

        public float Speed => _speed;
    }
}
