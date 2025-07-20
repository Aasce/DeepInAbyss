using UnityEngine;

namespace Asce.Game.Stats
{
    public interface IHasDefense : IGameObject
    {
        public DefenseGroupStats DefenseGroup { get; }
    }
}
