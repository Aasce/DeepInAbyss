using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Asce.Manager.Sounds
{
    [CreateAssetMenu(menuName = "Asce/Audio/Audio Data", fileName = "Audio Data")]
    public class SO_AudioData : ScriptableObject
    {
        [SerializeField] protected List<SO_AudioParameters> _audio = new();
        protected ReadOnlyCollection<SO_AudioParameters> _readonlyAudio;
        protected Dictionary<string, SO_AudioParameters> _audioDictionary;

        public ReadOnlyCollection<SO_AudioParameters> Audio => _readonlyAudio ??= _audio.AsReadOnly();

        public SO_AudioParameters Get(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (_audioDictionary == null) this.InitDictionary();
            if (!_audioDictionary.TryGetValue(name, out SO_AudioParameters parameter)) return null;
            return parameter;
        }

        protected virtual void InitDictionary()
        {
            _audioDictionary = new();
            foreach (SO_AudioParameters parameter in _audio)
            {
                if (parameter ==  null) continue;
                if (parameter.Clip == null) continue;

                if (_audioDictionary.ContainsKey(parameter.Name)) continue;
                _audioDictionary[parameter.Name] = parameter;
            }
        }
    }
}