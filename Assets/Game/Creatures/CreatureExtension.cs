using Asce.Game.Entities.Enemies;
using Asce.Game.Players;
using Asce.Game.Stats;
using Asce.Game.UIs;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities
{
    public static class CreatureExtension
    {
        /// <summary>
        ///     Check if creature is controlled by <see cref="Player"/>.
        /// </summary>
        /// <returns> 
        ///     Returns true if creature is controlled by <see cref="Player"/>.
        /// </returns>
        public static bool IsControlByPlayer(this ICreature creature)
        {
            return Player.Instance.ControlledCreature == creature;
        }

        public static void ControlledByPlayer(this ICreature creature)
        {
            if (creature == null) return;
            creature.IsControled = true;
            if (creature.Inventory != null) creature.Inventory.PickItemEnable = true;
            if (creature.UI != null) creature.UI.MainUI.SetUIForPlayer(creature);
            if (creature.View != null) creature.View.Layer = LayerUtils.LayerToInt(Player.Instance.Settings.RenderCreatureLayer);
            if (creature.Equipment is IHasWeaponSlot hasWeaponSlot)
            {
                if (hasWeaponSlot.WeaponSlot.CurrentWeapon != null)
                {
                    hasWeaponSlot.WeaponSlot.CurrentWeapon.View.Layer = LayerUtils.LayerToInt(Player.Instance.Settings.RenderCreatureLayer);
                }
                hasWeaponSlot.WeaponSlot.OnWeaponChanged += Creature_WeaponSlot_OnWeaponChanged;
            }
            creature.OnAfterTakeDamage += Creature_OnAfterTakeDamage;
        }
        public static void UncontrolledByPlayer(this ICreature creature)
        {
            if (creature == null) return;
            creature.IsControled = false;
            if (creature.Inventory != null) creature.Inventory.PickItemEnable = false;
            if (creature.UI != null) creature.UI.MainUI.SetUIForPlayer(creature);
            if (creature.View != null) creature.View.Layer = LayerMask.NameToLayer("Default");
            if (creature.Equipment is IHasWeaponSlot hasWeaponSlot)
            {
                if (hasWeaponSlot.WeaponSlot.CurrentWeapon != null)
                {
                    hasWeaponSlot.WeaponSlot.CurrentWeapon.View.Layer = LayerMask.NameToLayer("Default");
                }
                hasWeaponSlot.WeaponSlot.OnWeaponChanged -= Creature_WeaponSlot_OnWeaponChanged;
            }
            creature.OnAfterTakeDamage -= Creature_OnAfterTakeDamage;
        }

        private static void Creature_OnAfterTakeDamage(object sender, Combats.DamageContainer args)
        {
            Creature creature = (Creature)sender;
            if (creature.Stats is not IHasHealth hasHealth) return;

            if (hasHealth.HealthGroup.Health.Ratio > 0.4f) return;
            VFXs.VFXsManager.Instance.FullScreenVFXController.Set("Take Damage");
        }

        private static void Creature_WeaponSlot_OnWeaponChanged(object sender, Managers.ValueChangedEventArgs<Equipments.Weapons.WeaponObject> args)
        {
            if (args.OldValue != null) args.OldValue.View.Layer = LayerMask.NameToLayer("Default");
            if (args.NewValue != null) args.NewValue.View.Layer = LayerUtils.LayerToInt(Player.Instance.Settings.RenderCreatureLayer);
        }

    }
}