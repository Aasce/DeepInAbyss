using System;
using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IJumpable : ICreatureAction
    {
        public event Action<object> OnJump;

        public bool IsJumpEnabled { get; set; }
        public bool IsJumping { get; }
        public float JumpForce { get; }

        public void Jumping(bool isJump);
    }
}
