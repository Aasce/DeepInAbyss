using Asce.Managers;
using Asce.Managers.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace Asce.Game.Enviroments.Zones
{
    [RequireComponent(typeof(Collider2D))]
    public class SubZone : GameComponent
    {
        [SerializeField] protected Collider2D _zoneCollider;
        [SerializeField] protected List<Component> _components = new();
        protected ReadOnlyCollection<IOptimizedComponent> _optimizeComponents;

        public Collider2D ZoneCollider => _zoneCollider;
        public ReadOnlyCollection<IOptimizedComponent> OptimizedComponents => _optimizeComponents ??= _components.OfType<IOptimizedComponent>().ToList().AsReadOnly();

        protected override void Reset()
        {
            base.Reset();
            this.LoadOptimizedComponents();
        }
        protected override void RefReset()
        {
            base.RefReset();
            this.LoadComponent(out _zoneCollider);
        }

        protected virtual void Awake()
        {
            
        }

        protected List<Component> LoadOptimizedComponents()
        {
            if (_components == null) _components = new();
            else _components.Clear();

            _components.AddRange(transform.GetComponentsInChildren<Component>().Where((comp) => comp is IOptimizedComponent));
            return _components;
        }
    }
}
