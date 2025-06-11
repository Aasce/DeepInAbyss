using System;
using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IDodgeable : ICreatureAction
    {
        public event Action<object> OnDodgeStart;
        public event Action<object> OnDodgeEnd;

        public bool IsDodgeEnabled { get; set; }
        public bool IsDodging { get; }
        public int DodgeDirection { get; set; }

        public void Dodging();
    }
}
