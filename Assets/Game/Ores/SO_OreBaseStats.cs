using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Entities
{
    [CreateAssetMenu(menuName = "Asce/Entities/Ore Base Stats", fileName = "Ore Base Stats")]
    public class SO_OreBaseStats : SO_EntityBaseStats, IBaseStatsData
    {
        [Header("Health")]
        [SerializeField, Min(0f)] protected float _maxHealth;

        [Header("Defense")]
        [SerializeField] protected float _armor;
        [SerializeField] protected float _resistance;
        [SerializeField, Min(0f)] protected float _shield;


        public float MaxHealth => _maxHealth;

        public float Armor => _armor;
        public float Resistance => _resistance;
        public float Shield => _shield;
    }
}