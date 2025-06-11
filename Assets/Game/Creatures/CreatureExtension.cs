using Asce.Game.Entities.Enemies;
using Asce.Game.Players;
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
            creature.UI.MainUI.SetUIForPlayer(creature);
        }

        public static void UncontrolledByPlayer(this ICreature creature)
        {
            if (creature == null) return;
            creature.IsControled = false;
            creature.UI.MainUI.SetUIForPlayer(creature);
        }
    }
}