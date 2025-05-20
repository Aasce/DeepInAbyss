using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IClimbable : IMovable
    {
        public bool IsClimbing { get; set; }
        public void Climb(Vector2 direction);
    }
}
