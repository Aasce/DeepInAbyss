using Asce.Game.Entities;
using System;
using UnityEngine;

namespace Asce.Game.Stats
{
    public interface IHasJumpForce
    {
        public JumpForceStat JumpForce { get; }
    }
}
