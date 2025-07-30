using Asce.Game.Entities.Enemies;
using Asce.Game.Players;
using Asce.Game.Stats;
using Asce.Game.UIs;

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
            creature.OnAfterTakeDamage += Creature_OnAfterTakeDamage;
        }

        public static void UncontrolledByPlayer(this ICreature creature)
        {
            if (creature == null) return;
            creature.IsControled = false;
            if (creature.Inventory != null) creature.Inventory.PickItemEnable = false;
            if (creature.UI != null) creature.UI.MainUI.SetUIForPlayer(creature);
            creature.OnAfterTakeDamage -= Creature_OnAfterTakeDamage;
        }

        private static void Creature_OnAfterTakeDamage(object sender, Combats.DamageContainer args)
        {
            Creature creature = (Creature)sender;
            if (creature.Stats is not IHasHealth hasHealth) return;

            if (hasHealth.HealthGroup.Health.Ratio > 0.4f) return;
            VFXs.VFXsManager.Instance.FullScreenVFXController.Set("Take Damage");
        }
    }
}