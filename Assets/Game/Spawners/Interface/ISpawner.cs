using UnityEngine;

namespace Asce.Game.Spawners
{
    public interface ISpawner<T>
    {
        public T Spawn();
        public T Spawn(Vector2 position);
        public void Despawn(T instance);
    }
}