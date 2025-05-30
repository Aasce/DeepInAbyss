using Asce.Game.Entities;
using System;
using UnityEngine;

namespace Asce.Game.Stats
{
    public interface IHasHealth
    {
        public bool IsDead { get; }
        public HealthGroupStats HealthGroup { get; }
    }
}
