using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities
{
    public interface ICreature : IEntity, 
        IHasView<CreatureView>, IHasUI<CreatureUI>, IHasAction<CreatureAction>, IHasStats<CreatureStats, SO_CreatureBaseStats>, 
        IHasEquipment<CreatureEquipment>, IHasInventory<CreatureInventory>, IHasSpoils<CreatureSpoils>
    {
        public bool IsControled { get; set; }
    }
}
