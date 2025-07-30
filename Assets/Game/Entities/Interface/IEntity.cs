using Asce.Managers;
using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IEntity : IGameObject
    {
        public SO_EntityInformation Information { get; }
        public EntityStatus Status { get; }
    }
}
