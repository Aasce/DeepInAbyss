using Asce.Managers;
using System;
using UnityEngine;

namespace Asce.Game.Enviroments
{
    public abstract class InteractiveObject : GameComponent, IInteractableObject
    {
        [SerializeField, Min(0f)] protected float _interactionRange = 0f;
        [SerializeField] protected Vector2 _offset = Vector2.zero;

        public event Action<object> OnFocus;
        public event Action<object> OnUnfocus;

        public virtual float InteractionRange => _interactionRange;
        public virtual Vector2 Offset => _offset;

        public abstract void Interact(GameObject interactor);

        public virtual void Focus()
        {
            OnFocus?.Invoke(this);
        }

        public virtual void Unfocus()
        {
            OnUnfocus?.Invoke(this);
        }
    }
}