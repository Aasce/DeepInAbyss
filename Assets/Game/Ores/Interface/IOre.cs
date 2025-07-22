using Asce.Game.Combats;

namespace Asce.Game.Entities.Ores
{
    public interface IOre : IEntity, IHasUI<OreUI>, 
        IHasStats<OreStats, SO_OreBaseStats>, ITakeDamageable
    {

    }
}