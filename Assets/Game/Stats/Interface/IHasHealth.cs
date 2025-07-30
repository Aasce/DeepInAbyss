using Asce.Managers;
using System;
using UnityEngine;

namespace Asce.Game.Stats
{
    public interface IHasHealth : IGameObject
    {
        public bool IsDead { get; }
        public HealthGroupStats HealthGroup { get; }
    }
}
