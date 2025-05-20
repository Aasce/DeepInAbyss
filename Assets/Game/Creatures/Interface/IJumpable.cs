using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IJumpable : IMovable
    {
        public bool CanJump { get; }
        public Cooldown JumpCooldown { get; set; }

        public void Jump();
    }
}
