using Asce.Game.Enviroments;
using Asce.Managers;
using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Entities.Characters
{
    public class CharacterInteraction : GameComponent, IHasOwner<Character>
    {
        [SerializeField, Readonly] protected Character _owner;

        [Space]
        [SerializeField] protected Cooldown _checkCooldown = new(0.2f);
        [SerializeField] protected float _checkRadius = 10f;
        [SerializeField] protected LayerMask _interactiveLayer;

        protected readonly HashSet<IInteractableObject> _interactableObjects = new();
        protected IInteractableObject _focusObject;
        
        protected List<Collider2D> _results = new();
        protected HashSet<IInteractableObject> _toRemove = new();

        protected ContactFilter2D _filter;

        public event Action<object, IInteractableObject> OnInteractableObjectAdded;
        public event Action<object, IInteractableObject> OnInteractableObjectRemoved;

        public IEnumerable<IInteractableObject> InteractableObjects => _interactableObjects;
        public IInteractableObject FocusObject => _focusObject;

        public Character Owner
        {
            get => _owner;
            set => _owner = value;
        }

        private void Start()
        {
            _filter = new ContactFilter2D()
            {
                useLayerMask = true,
                layerMask = _interactiveLayer,
                useTriggers = true
            };
        }

        protected virtual void Update()
        {
            _checkCooldown.Update(Time.deltaTime);
            if (_checkCooldown.IsComplete)
            {
                CheckInteractiveObjects();
                _checkCooldown.Reset();
            }
        }

        public virtual void FocusAt(Vector2 point)
        {
            _focusObject = this.GetObjectNearest(point);
        }

        public IInteractableObject GetObjectNearest(Vector2 point)
        {
            if (_interactableObjects.Count <= 0) return null;

            float minDistance = float.PositiveInfinity;
            IInteractableObject nearest = null;

            foreach (IInteractableObject obj in _interactableObjects)
            {
                if (obj == null) continue;
                Vector2 interactableObjectPosition = (Vector2)obj.gameObject.transform.position + obj.Offset;
                float distance = Vector2.Distance(point, interactableObjectPosition);

                if (distance < minDistance) 
                {
                    minDistance = distance;
                    nearest = obj;
                }
            }

            return nearest;
        }

        protected void CheckInteractiveObjects()
        {
            _toRemove.Clear(); 
            _toRemove.UnionWith(_interactableObjects);

            Vector2 center = (Vector2)_owner.transform.position + Vector2.up * Owner.Status.Height * 0.5f;
            Physics2D.OverlapCircle(center, _checkRadius, _filter, _results);

            foreach (var collider in _results)
            {
                if (!collider || !collider.TryGetComponent(out IInteractableObject obj)) continue;

                Vector2 interactableObjectPosition = (Vector2)obj.gameObject.transform.position + obj.Offset;
                float distance = Vector2.Distance(center, interactableObjectPosition);

                if (distance <= obj.InteractionRange)
                {
                    _toRemove.Remove(obj);

                    if (_interactableObjects.Add(obj))
                        OnInteractableObjectAdded?.Invoke(_owner, obj);
                }
            }

            foreach (var obj in _toRemove)
            {
                _interactableObjects.Remove(obj);
                OnInteractableObjectRemoved?.Invoke(_owner, obj);
            }
        }


    }
}
