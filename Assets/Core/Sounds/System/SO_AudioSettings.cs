using UnityEngine;

namespace Asce.Manager.Sounds
{
    [CreateAssetMenu(menuName = "Asce/Audio/Audio Settings", fileName = "Audio Settings")]
    public class SO_AudioSettings : ScriptableObject
    {
        public const float MAX_VOLUME = 2f;

        [SerializeField, Range(0f, 2f)] protected float _masterVolume = 1f;
        [SerializeField, Range(0f, 2f)] protected float _musicVolume = 1f;
        [SerializeField, Range(0f, 2f)] protected float _sfxVolume = 1f;

        public float MasterVolume
        {
            get => _masterVolume;
            set
            {
                _masterVolume = Mathf.Clamp(value, 0f, MAX_VOLUME);
            }
        }


        public float MusicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = Mathf.Clamp(value, 0f, MAX_VOLUME);
            }
        }


        public float SFXVolume
        {
            get => _sfxVolume;
            set
            {
                _sfxVolume = Mathf.Clamp(value, 0f, MAX_VOLUME);
            }
        }
    }
}