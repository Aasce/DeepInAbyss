using System;
using UnityEngine;

namespace Asce.Game.Entities
{
    public interface ICrouchable : ICreatureAction
    {
        public event Action<object> OnCrouchStart;
        public event Action<object> OnCrouchEnd;

        public bool IsCrouching { get; }
        public float CrouchMaxSpeed {  get; }
        public float CrouchAcceleration {  get; }

        public void Crouching();
    }
}
