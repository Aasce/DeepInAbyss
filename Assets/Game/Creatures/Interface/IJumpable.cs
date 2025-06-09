using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IJumpable : IEntity
    {
        public bool IsJumpEnabled { get; set; }
        public bool IsJumping { get; }
        public float JumpForce { get; }

        public void Jumping(bool isJump);
    }
}
