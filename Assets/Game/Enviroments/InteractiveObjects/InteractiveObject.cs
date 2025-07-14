using Asce.Managers;
using UnityEngine;

namespace Asce.Game.Enviroments
{
    public abstract class InteractiveObject : GameComponent, IInteractableObject
    {
        [SerializeField, Min(0f)] protected float _interactionRange = 0f;
        [SerializeField] protected Vector2 _offset = Vector2.zero;

        public virtual float InteractionRange => _interactionRange;
        public virtual Vector2 Offset => _offset;

        public abstract void Interact(GameObject interactor);

        public virtual void OnFocusEnter()
        {

        }

        public virtual void OnFocusExit()
        {

        }
    }
}