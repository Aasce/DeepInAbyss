using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IHasOwner<T> where T : IEntity
    {
        T Owner { get; set; }
    }
}
