using Asce.Managers.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Asce.Game.Enviroments.Zones
{
    [RequireComponent(typeof(Collider2D))]
    public class Zone : MonoBehaviour
    {
        [SerializeField] protected Collider2D _zoneCollider;
        [SerializeField] protected List<SubZone> _subZones = new();

        protected ReadOnlyCollection<SubZone> _readonlySubZones;

        public Collider2D ZoneCollider => _zoneCollider;
        public ReadOnlyCollection<SubZone> SubZones => _readonlySubZones  ??= _subZones.AsReadOnly();


        protected virtual void Reset()
        {
            this.LoadComponent(out  _zoneCollider);
            _subZones.Clear();
            _subZones.AddRange(transform.GetComponentsInChildren<SubZone>());
        }
    }
}
