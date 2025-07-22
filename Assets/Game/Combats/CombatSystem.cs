using Asce.Game.Entities;
using Asce.Game.Entities.AIs;
using Asce.Game.Entities.Ores;
using Asce.Game.FloatingTexts;
using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Combats
{
    /// <summary>
    ///     Handles core combat logic, including damage calculation, defense handling, shield absorption, and healing.
    /// </summary>
    public class CombatSystem
    {
        /// <summary>
        ///     A constant value used to balance the damage reduction formula.
        /// </summary>
        public static readonly float deductDamageBalance = 100f;

        /// <summary>
        ///     Calculates and applies damage from a sender to a receiver, accounting for defenses, penetration, and shields.
        /// </summary>
        /// <param name="container"> The container holding all relevant damage information. </param>
        public static void DamageDealing(DamageContainer container)
        {
            if (container == null) return;
            if (container.Sender == null || container.Receiver == null) return;
            if (container.Sender.IsDead ||  container.Receiver.IsDead) return;

            container.Sender.BeforeSendDamage(container);
            container.Receiver.BeforeTakeDamage(container);

            if (container.Receiver is ICreature) DamageDealingCreature(container);
            else if (container.Receiver is IOre) DamageDealingOre(container);

            container.Receiver.AfterTakeDamage(container);
            container.Sender.AfterSendDamage(container);
        }

        private static void DamageDealingCreature(DamageContainer container)
        {
            if (container.Receiver is not ICreature entity) return;
            IHasDefense hasDefense = entity.Stats;
            IHasHealth hasHealth = entity.Stats;

            DamageDealingEntity(container, hasDefense, hasHealth);
        }

        private static void DamageDealingOre(DamageContainer container)
        {
            if (container.Receiver is not IOre ore) return;
            IHasDefense hasDefense = ore.Stats;
            IHasHealth hasHealth = ore.Stats;

            DamageDealingEntity(container, hasDefense, hasHealth);
        }

        private static void DamageDealingEntity(DamageContainer container, IHasDefense hasDefense, IHasHealth hasHealth)
        {
            float defense = GetDefense(hasDefense, container.DamageType);
            float finalDefense = DefenseAfterPenetration(defense, container.Penetration, container.PenetrationType);

            float damageDeal = container.Damage * DeductDamageRatio(finalDefense);

            float finalDamage = DamageAffterShield(hasDefense, damageDeal);
            float absorbedByShield = damageDeal - finalDamage;

            container.FinalDamage = finalDamage;
            SendDamage(container.Sender, hasHealth, finalDamage);

            if (absorbedByShield > 0)
            {
                StatValuePopupManager.Instance.CreateShieldAbsorptionPopupText(absorbedByShield, container.Position);
                if (finalDamage > 0) StatValuePopupManager.Instance.CreateDamagePopupText(container.FinalDamage, container.DamageType, container.Position, 0.25f);
            }
            else StatValuePopupManager.Instance.CreateDamagePopupText(container.FinalDamage, container.DamageType, container.Position);
        }

        /// <summary>
        ///     Gets the base defense value of the receiver based on the damage type.
        /// </summary>
        /// <param name="receiver"> The target entity taking damage. </param>
        /// <param name="type"> The type of damage being applied. </param>
        /// <returns>
        ///     The appropriate defense value.
        /// </returns>
        public static float GetDefense(IHasDefense receiver, DamageType type)
        {
            if (receiver == null) return 0f;
            return type switch
            {
                DamageType.TrueDamage => 0f,
                DamageType.Physical => receiver.DefenseGroup.Armor.Value,
                DamageType.Magical => receiver.DefenseGroup.Resistance.Value,
                _ => 0f,
            };
        }

        /// <summary>
        ///     Calculates the effective defense after applying penetration.
        /// </summary>
        /// <param name="defence"> The base defense value. </param>
        /// <param name="penetration"> The amount of penetration applied. </param>
        /// <param name="penetrationType"> The type of penetration (flat or ratio). </param>
        /// <returns>
        ///     The adjusted defense value.
        /// </returns>
        public static float DefenseAfterPenetration(float defence, float penetration, StatValueType penetrationType)
        {
            if (defence <= 0f) return 0f;
            if (penetration <= 0f) return defence;

            float effectiveDefense = penetrationType switch
            {
                StatValueType.Plat => defence - penetration,
                StatValueType.Ratio => defence - (defence * penetration),
                _ => defence,
            };

            return Mathf.Max(effectiveDefense, 0f);
        }

        /// <summary>
        ///     Calculates the damage reduction ratio based on the defender’s defense.
        /// </summary>
        /// <param name="defence"> The adjusted defense value. </param>
        /// <returns>
        ///     The damage reduction ratio (a value between 0 and 1).
        /// </returns>
        public static float DeductDamageRatio(float defence)
        {
            if (defence <= 0f) return 1f;
            return deductDamageBalance / (deductDamageBalance + defence);
        }

        /// <summary>
        ///     Calculates remaining damage after accounting for shield absorption.
        /// </summary>
        /// <param name="receiver"> The target entity taking damage. </param>
        /// <param name="damage"> The initial damage to apply. </param>
        /// <returns> The damage that passes through after the shield absorbs as much as possible. </returns>
        public static float DamageAffterShield(IHasDefense receiver, float damage)
        {
            if (receiver == null) return damage;

            float shieldValue = receiver.DefenseGroup.Shield.CurrentValue;
            if (shieldValue <= 0f) return damage;

            float remainingDamage = damage - shieldValue;
            if (remainingDamage > 0f)
            {
                receiver.DefenseGroup.Shield.SetCurrentValue(receiver.gameObject, "Shield is depleted", 0f);
                return remainingDamage;
            }
            else
            {
                receiver.DefenseGroup.Shield.AddToCurrentValue(receiver.gameObject, "Reduce shield value", -damage);
                return 0f;
            }
        }

        /// <summary>
        ///     Heals the receiver by a specified amount, scaled by any healing modifiers.
        /// </summary>
        /// <param name="healer"> The entity providing the healing. </param>
        /// <param name="receiver"> The entity receiving the healing. </param>
        /// <param name="heal"> The amount of healing to apply. </param>
        /// <param name="type"> The type of stat value (flat or ratio-based). </param>
        /// <returns>
        ///     Returns final heal value.
        /// </returns>
        public static float Healing(IEntity healer, IHasHealth receiver, Vector2 position, float heal, StatValueType type = StatValueType.Plat)
        {
            if (receiver == null) return 0f;
            if (receiver.IsDead) return 0f;

            float healValue = receiver.HealthGroup.Heal(healer, "Healing", heal, type);

            StatValuePopupManager.Instance.CreateHealPopupText(healValue, position);
            return healValue;
        }

        /// <summary>
        ///     Applies final damage to the receiver's health after all calculations.
        /// </summary>
        /// <param name="sender"> The entity that dealt the damage. </param>
        /// <param name="receiver"> The entity receiving the damage. </param>
        /// <param name="damage"> The final damage amount to apply. </param>
        protected static void SendDamage(ISendDamageable sender, IHasHealth receiver, float damage)
        {
            if (sender == null || receiver == null) return;
            receiver.HealthGroup.Health.AddToCurrentValue(sender.gameObject, "Damage Dealt", -damage);
        }
    }
}
