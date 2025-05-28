using UnityEngine;

namespace Asce.Game.Stats
{
    public interface IStatsController<T> where T : IBaseStatsData
    {
        T BaseStats { get; }

        public void LoadBaseStats();
        public void UpdateStats(float deltaTime);
        public void ClearStats(bool isForceClear = false);
        public void ResetStats();
    }
}