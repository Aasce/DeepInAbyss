using UnityEngine;

namespace Asce.Manager.Sounds
{
    [CreateAssetMenu(menuName = "Asce/Audio/Audio Parameters", fileName = "Audio Parameters")]
    public class SO_AudioParameters : ScriptableObject
    {
        [SerializeField] protected string _name;
        [SerializeField] protected AudioClip _clip;

        [Header("Parameters")]
        [SerializeField] protected bool _loop = false;
        [SerializeField] protected bool _playOnAwake = true;

        [SerializeField, Range(0f, 1f)] protected float _volume = 1.0f;
        [SerializeField, Range(-3f, 3f)] protected float _pitch = 1.0f;

        [Space]
        [SerializeField, Range(0f, 1f)] protected float _spatialBlend = 1.0f;
        [SerializeField] protected AudioRolloffMode _volumeRolloff = AudioRolloffMode.Logarithmic;
        [SerializeField, Min(0f)] protected float _minDistance = 1.0f;
        [SerializeField, Min(0f)] protected float _maxDistance = 1.0f;


        public string Name => _name;
        public AudioClip Clip => _clip;

        public bool Loop => _loop;
        public bool PlayOnAwake => _playOnAwake;

        public float Volume => _volume;
        public float Pitch => _pitch;

        public float SpatialBlend => _spatialBlend;
        public AudioRolloffMode VolumeRolloff => _volumeRolloff;
        public float MinDistance => _minDistance;
        public float MaxDistance => _maxDistance;
    }
}
