using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    [CreateAssetMenu(menuName = "Asce/Status Effects/Status Effect Data", fileName = "Status Effect Data")]
    public class SO_StatusEffectData : ScriptableObject
    {
        [SerializeField] protected List<SO_StatusEffectInformation> _data = new();
        protected Dictionary<string, SO_StatusEffectInformation> _dataDictionary;

        public Dictionary<string, SO_StatusEffectInformation> Dictionary => this.GetDictionary(); 

        public SO_StatusEffectInformation GetInformationByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (!this.Dictionary.ContainsKey(name)) return null;

            return this.Dictionary[name];
        }

        public Dictionary<string, SO_StatusEffectInformation> GetDictionary()
        {
            if (_dataDictionary == null)
            {
                _dataDictionary = new();
                foreach(SO_StatusEffectInformation information in _data)
                {
                    if (information == null) continue;
                    if (_dataDictionary.ContainsKey(information.Name)) continue;

                    _dataDictionary[information.Name] = information;
                }
            }
            return _dataDictionary;
        }
    }
}
