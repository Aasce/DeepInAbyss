using UnityEngine;

namespace Asce.Game.Stats
{
    [System.Serializable]
    public class Stat : BaseStat<StatAgent>
    {
        public Stat() : base() { }
        public Stat(StatType type) : base(type) { }

        protected override StatAgent CreateAgent(GameObject author, string reason, float value, StatValueType type, float duration, Vector2 position)
        {
            return new StatAgent(author, reason, value, type, duration, position);
        }
    }
}