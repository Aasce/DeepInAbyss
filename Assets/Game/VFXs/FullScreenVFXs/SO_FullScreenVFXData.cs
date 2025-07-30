using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.VFXs
{
    [CreateAssetMenu(menuName = "Asce/VFXs/Full Screen VFX Data", fileName = "Full Screen VFX Data")]
    public class SO_FullScreenVFXData : ScriptableObject
    {
        [SerializeField] protected List<SO_FullScreenVFXParameters> _fullScreenParameters = new();
        protected Dictionary<string, SO_FullScreenVFXParameters> _parametersDictionary;

        public Dictionary<string, SO_FullScreenVFXParameters> Parameters => _parametersDictionary;

        public SO_FullScreenVFXParameters GetParameterByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (_parametersDictionary == null) this.InitDicrionary();
            
            if (_parametersDictionary.TryGetValue(name, out SO_FullScreenVFXParameters parameter))
            {
                return parameter;
            }

            Debug.LogWarning($"Full Screen VFX Data not has parameter with name \"{name}\"");
            return null;
        }

        protected virtual void InitDicrionary()
        {
            _parametersDictionary = new Dictionary<string, SO_FullScreenVFXParameters>();
            foreach (var parameter in _fullScreenParameters)
            {
                if (parameter != null && !_parametersDictionary.ContainsKey(parameter.Name))
                {
                    _parametersDictionary.Add(parameter.Name, parameter);
                }
            }
        }
    }
}