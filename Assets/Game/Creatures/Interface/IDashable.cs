using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IDashable : IEntity
    {
        public bool IsDashEnabled { get; set; }
        public bool IsDashing { get; }

        public float DashStartSpeed { get; }
        public float DashMaxSpeed { get; }
        public float DashAcceleration { get; }

        public void Dashing();
    }
}
