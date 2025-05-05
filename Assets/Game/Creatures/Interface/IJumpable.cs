using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IJumpable : IMovable
    {
        public bool CanJump { get; set; }
        public float JumpCooldown { get; set; }

        public void Jump();
    }
}
