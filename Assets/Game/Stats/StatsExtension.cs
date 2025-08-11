using Asce.Game.Combats;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Stats
{
    public static class StatsExtenstion
    {
        public static string GetContent(this Stat stat)
        {
            if (stat == null) return string.Empty;
            switch (stat.StatType) 
            {
                case StatType.Armor:
                    string physicalDamgeType = "Physical Damage".ColorWrap("#FFA500");
                    return $"Reduct {(1f - CombatSystem.DeductDamageRatio(stat.Value)) * 100f:#.#}% {physicalDamgeType}";

                case StatType.Resistance:
                    string magicalDamgeType = "Magical Damage".ColorWrap("#DDA0DD");
                    return $"Reduct {(1f - CombatSystem.DeductDamageRatio(stat.Value)) * 100f:#.#}% {magicalDamgeType}";

                case StatType.Shield:
                    if (stat is ResourceStat resourceStat)
                        return $"Block {resourceStat.CurrentValue:#.#} {"Damage".ColorWrap(Color.yellow)}";
                    else return string.Empty;

                case StatType.None:
                case StatType.Health:
                case StatType.HealthScale:
                case StatType.Stamina:
                case StatType.Hunger:
                case StatType.Thirst:
                case StatType.Breath:
                case StatType.Strength:
                case StatType.Speed:
                case StatType.JumpForce:
                case StatType.ViewRadius:
                default:
                    return string.Empty;
            }
        }
    }
}