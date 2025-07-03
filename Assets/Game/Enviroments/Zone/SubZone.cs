using Asce.Managers.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Asce.Game.Enviroments.Zones
{
    [RequireComponent(typeof(Collider2D))]
    public class SubZone : MonoBehaviour
    {
        [SerializeField] protected Collider2D _zoneCollider;
        [SerializeField] protected List<Component> _optimizedComponents = new();

        public Collider2D ZoneCollider => _zoneCollider;
        public List<Component> OptimizedComponents => _optimizedComponents;

        protected virtual void Reset()
        {
            this.LoadComponent(out _zoneCollider);
            this.LoadOptimizedComponents();
        }

        protected virtual void Awake()
        {
            
        }

        protected List<Component> LoadOptimizedComponents()
        {
            if (_optimizedComponents == null) _optimizedComponents = new();
            else _optimizedComponents.Clear();

            _optimizedComponents.AddRange(transform.GetComponentsInChildren<Component>().Where((comp) => comp is IOptimizedComponent));
            return _optimizedComponents;
        }
    }
}
