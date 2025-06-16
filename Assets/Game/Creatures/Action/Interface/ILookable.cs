using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities
{
    public interface ILookable : ICreatureAction
    {
        public bool IsLooking { get; }
        public Vector2 TargetPosition { get; }

        public void Looking(bool isLooking, Vector2 target = default);
    }
}
