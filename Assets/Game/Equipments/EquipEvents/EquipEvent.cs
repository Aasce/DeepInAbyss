using Asce.Game.Entities;
using Asce.Game.Stats;
using Asce.Managers;
using UnityEngine;

namespace Asce.Game.Equipments.Events
{
    public abstract class EquipEvent : GameComponent
    {
        [SerializeField] protected float _deductDurabilityScale = 0.2f;

        public float DeductDurabilityScale => _deductDurabilityScale;

        public abstract void OnEquip(ICreature creature);
        public abstract void OnUnequip(ICreature creature);
        public abstract string GetDescription(bool isPretty = false);


        protected string GenerateDescription(bool isPretty, params StatValue[] statValues)
        {
            string description = string.Empty;

            foreach (StatValue statValue in statValues)
            {
                bool isPositive = statValue.Value >= 0f;

                string valueStr = statValue.ValueType switch
                {
                    StatValueType.Ratio => $"{(isPositive ? "+" : "-")}{Mathf.Abs(statValue.Value * 100f):0.#}%",
                    StatValueType.Scale => $"*{statValue.Value:0.##}",
                    StatValueType.Base or StatValueType.Flat => $"{(isPositive ? "+" : "-")}{Mathf.Abs(statValue.Value):0.#}",
                    _ => statValue.Value.ToString()
                };

                if (isPretty)
                {
                    string color = isPositive ? "green" : "red";
                    valueStr = $"<color={color}>{valueStr}</color>";
                }

                description += $"{valueStr} {statValue.Type}\n";
            }

            return description;
        }
    }
}
