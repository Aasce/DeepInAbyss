using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.VFXs
{
    [CreateAssetMenu(menuName = "Asce/VFXs/Status Effect VFXs Data", fileName = "Status Effect VFXs")]
    public class SO_StatusEffectVFXs : ScriptableObject
    {
        [SerializeField] protected List<StatusEffectVFXContainer> _vfxs = new();

        public void Register(string name)
        {
            if (string.IsNullOrEmpty(name)) return;

            StatusEffectVFXContainer container = _vfxs.Find((c) => c != null && string.Equals(c.Name, name));
            if (container == null) return;
            if (container.Prefab == null) return;

            VFXsManager.Instance.Register(container.Prefab);
        }
    }

    [System.Serializable]
    public class StatusEffectVFXContainer
    {
        [SerializeField] private string _name;
        [SerializeField] private VFXObject _vfxPrefab;

        public string Name => _name;
        public VFXObject Prefab => _vfxPrefab;
    }
}