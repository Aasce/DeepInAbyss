using UnityEngine;

namespace Asce.Game.Stats
{
    [System.Serializable]
    public class Stat : BaseStat<StatAgent>
    {
        protected override StatAgent CreateAgent(GameObject author, string reason, float value, StatValueType type, float duration, Vector2 position)
        {
            return new StatAgent(author, reason, value, type, duration, position);
        }
    }
}