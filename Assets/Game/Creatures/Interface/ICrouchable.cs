using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities
{
    public interface ICrouchable : IEntity
    {
        public bool IsCrouching { get; }
        public float CrouchMaxSpeed {  get; }
        public float CrouchAcceleration {  get; }

        public void Crouching();
    }
}
