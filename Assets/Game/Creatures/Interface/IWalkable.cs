using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IWalkable : IMovable
    {
        public void Walk(float direction);
    }
}
