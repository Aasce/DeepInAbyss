using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IDodgeable : IEntity
    {
        public bool IsDodgeEnabled { get; set; }
        public bool IsDodging { get; }
        public int DodgeDirection { get; set; }

        public void Dodging();
    }
}
