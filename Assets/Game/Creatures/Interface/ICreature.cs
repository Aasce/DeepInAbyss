using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities
{
    public interface ICreature : IEntity, IHasView<CreatureView>, IHasAction<CreatureAction>, IHasStats<CreatureStats, SO_CreatureBaseStats>, IHasUI<CreatureUI>
    {
        public bool IsControled { get; set; }
    }
}
