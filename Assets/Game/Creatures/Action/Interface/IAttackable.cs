using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IAttackable : ICreatureAction
    {
        public bool IsAttacking { get; }

        public void Attacking(bool isAttacking);
    }
}
