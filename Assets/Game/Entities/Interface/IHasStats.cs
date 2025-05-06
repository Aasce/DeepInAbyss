using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IHasStats<TController, IStatsData> 
        where TController : IStatsController<IStatsData> 
        where IStatsData : IBaseStatsData
    {
        TController Stats { get; }
    }
}
