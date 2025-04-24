using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IClimbale : IMovable
    {
        public bool IsClimbing { get; set; }
        public void Climb(Vector2 direction);
    }
}
