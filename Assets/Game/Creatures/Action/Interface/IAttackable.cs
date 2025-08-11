using Asce.Game.Combats;
using System;
using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IAttackable : ICreatureAction
    {
        public event Action<object, AttackEventArgs> OnAttackStart;
        public event Action<object, AttackEventArgs> OnAttackHit;
        public event Action<object, AttackEventArgs> OnAttackEnd;

        public bool IsStartAttack { get; }

        public void Attacking(bool isAttacking);
        public void MeleeAttacking(bool isAttacking);
    }
}
