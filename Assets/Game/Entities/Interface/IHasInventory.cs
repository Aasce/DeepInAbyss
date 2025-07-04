using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IHasInventory<T> where T : class
    {
        T Inventory { get; }
    }
}