using System;
using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IThrowableWeapon : ICreatureAction
    {
        public float ThrowForce { get; set; }
        public float ThrowAngularSpeed { get; set; }


        public event Action<object> OnThrow;

        public void Throwing();
    }
}
