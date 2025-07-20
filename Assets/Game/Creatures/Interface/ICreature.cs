using Asce.Game.Combats;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities
{
    public interface ICreature : IEntity, 
        IHasView<CreatureView>, IHasUI<CreatureUI>, IHasAction<CreatureAction>, 
        IHasStats<CreatureStats, SO_CreatureBaseStats>, IHasStatusEffect<CreatureStatusEffect>, ISendDamageable, ITakeDamageable,
        IHasEquipment<CreatureEquipment>, IHasInventory<CreatureInventory>, IHasSpoils<CreatureSpoils>
    {
        public bool IsControled { get; set; }
    }
}
