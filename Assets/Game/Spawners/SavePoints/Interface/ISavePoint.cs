using Asce.Managers.SaveLoads;
using UnityEngine;

namespace Asce.Game.Spawners
{
    public interface ISavePoint : IGameObject, IUniqueIdentifiable, IReceiveData<bool>
    {
        public bool IsActive { get; set; }

        /// <summary> Gets the position of the save point. </summary>
        Vector2 Position => gameObject != null ? gameObject.transform.position : Vector2.zero;
    }
}