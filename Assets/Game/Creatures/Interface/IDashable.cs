using System;
using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IDashable : ICreatureAction
    {
        public event Action<object> OnDash;

        public bool IsDashEnabled { get; set; }
        public bool IsDashing { get; }

        public float DashStartSpeed { get; }
        public float DashMaxSpeed { get; }
        public float DashAcceleration { get; }

        public void Dashing();
    }
}
