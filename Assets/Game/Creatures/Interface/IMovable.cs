using System;
using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IMovable : ICreatureAction
    {
        public event Action<object> OnMoveStart;
        public event Action<object> OnMoveEnd;

        public bool IsMoveEnabled { get; set; }
        public bool IsMoving { get; }
        public bool IsIdle { get; }

        public void Moving(Vector2 direction);
    }
}
