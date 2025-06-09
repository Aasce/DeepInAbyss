using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IRunnable : IEntity
    {
        public float RunMaxSpeed { get; }
        public float RunAcceleration { get; }

        public void Running(bool state);
    }
}
