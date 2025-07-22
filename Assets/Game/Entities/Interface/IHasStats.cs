using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IHasStats<TController, IStatsData> : IEntity
        where TController : IStatsController<IStatsData> 
        where IStatsData : IBaseStatsData
    {
        TController Stats { get; }
    }

    public interface IHasStats : IHasStats<IStatsController<IBaseStatsData>, IBaseStatsData> { }
}
