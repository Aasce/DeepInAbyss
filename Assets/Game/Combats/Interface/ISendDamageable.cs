using Asce.Managers;
using System;
using UnityEngine;

namespace Asce.Game.Combats
{
    public interface ISendDamageable : IGameObject
    {
        bool IsDead { get; }

        public event Action<object, DamageContainer> OnBeforeSendDamage;
        public event Action<object, DamageContainer> OnAfterSendDamage;

        public void BeforeSendDamage(DamageContainer container);
        public void AfterSendDamage(DamageContainer container);
    }

}