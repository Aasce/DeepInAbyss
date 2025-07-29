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
        public event Action<object, GameObject> OnInteract;

        public virtual float InteractionRange => _interactionRange;
        public virtual Vector2 Offset => _offset;

        public virtual void Interact(GameObject interactor)
        {
            OnInteract?.Invoke(this, interactor);
        }

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