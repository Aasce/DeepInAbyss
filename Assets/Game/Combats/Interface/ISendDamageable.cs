using Asce.Game.Entities;
using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Combats
{
    public interface ISendDamageable : IGameObject, IHasCombatStats
    {
        public void BeforeSendDamage(DamageContainer container);
        public void AfterSendDamage(DamageContainer container);
    }

}