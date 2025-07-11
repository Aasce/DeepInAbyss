using Asce.Game.Entities;
using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Combats
{
    public interface ITakeDamageable : IGameObject, IHasOwner<Creature>, IHasCombatStats
    {
        public void BeforeTakeDamage(DamageContainer container);
        public void AfterTakeDamage(DamageContainer container);
    }

}