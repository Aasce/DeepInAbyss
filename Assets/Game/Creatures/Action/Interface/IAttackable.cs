using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IAttackable : ICreatureAction
    {
        public bool IsStartAttack { get; }

        public void Attacking(bool isAttacking);
        public void MeleeAttacking(bool isAttacking);
    }
}
