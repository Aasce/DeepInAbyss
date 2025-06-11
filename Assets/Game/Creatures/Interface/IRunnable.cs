using System;
using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IRunnable : ICreatureAction
    {
        public event Action<object> OnRunStart;
        public event Action<object> OnRunEnd;

        public float RunMaxSpeed { get; }
        public float RunAcceleration { get; }

        public void Running(bool state);
    }
}
