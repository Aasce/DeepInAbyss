using Asce.Managers;
using System;
using UnityEngine;

namespace Asce.Game.Combats
{
    public interface ITakeDamageable : IGameObject
    {
        bool IsDead { get; }

        public event Action<object, DamageContainer> OnBeforeTakeDamage;
        public event Action<object, DamageContainer> OnAfterTakeDamage;

        public void BeforeTakeDamage(DamageContainer container);
        public void AfterTakeDamage(DamageContainer container);
    }

}