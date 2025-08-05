using Asce.Managers;
using Asce.Managers.Pools;
using System.Collections;
using UnityEngine;

namespace Asce.Game.Sounds
{
    public class AudioManager : MonoBehaviourSingleton<AudioManager>
    {
        [SerializeField] protected SO_AudioData _audio;

        [Space]
        [SerializeField] protected AudioSource _musicSource;
        [SerializeField] protected Pool<AudioSource> _sfxPool = new();

        [Space]
        [SerializeField] private float _crossfadeDuration = 1.5f;
        [SerializeField] private float _crossSlientDuration = 0f;
        private Coroutine _crossfadeCoroutine;

        public AudioSource PlayMusic(string name)
        {
            SO_AudioParameters parameters = _audio.Get(name);
            if (parameters == null) return null;

            if (_musicSource == null) return null;

            Config(_musicSource, parameters);
            _musicSource.Play();
            return _musicSource;
        }

        public AudioSource PlaySFX(string name, Vector2 position = default, float delay = 0f)
        {
            SO_AudioParameters parameters = _audio.Get(name);
            if (parameters == null) return null;

            AudioSource source = _sfxPool.Activate();
            if (source == null) return null;

            this.Config(source, parameters);
            if (!parameters.Loop) this.AutoDeactive(source, _sfxPool);
            source.transform.position = position;
            StartCoroutine(this.PlayDelay(source, delay));
            return source;
        }


        public void CrossfadeMusic(string name, float? fadeDuration = null, float? slientDuration = null)
        {
            var parameters = _audio.Get(name);
            if (parameters == null) return;

            if (_musicSource.clip == parameters.Clip) return;

            if (_crossfadeCoroutine != null)
                StopCoroutine(_crossfadeCoroutine);

            float fadeTime = fadeDuration ?? _crossfadeDuration;
            float slientTime = slientDuration ?? _crossSlientDuration;
            _crossfadeCoroutine = StartCoroutine(CrossfadeRoutine(parameters, fadeTime, slientTime));
        }

        protected IEnumerator CrossfadeRoutine(SO_AudioParameters newMusic, float fadeDuration = 1.5f, float slientDuration = 0f)
        {
            float startVolume = _musicSource.volume;

            // Fade out
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                _musicSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
                yield return null;
            }

            Config(_musicSource, newMusic);
            if (slientDuration > 0) yield return new WaitForSeconds(slientDuration);
            _musicSource.Play();

            // Fade in
            t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                _musicSource.volume = Mathf.Lerp(0f, newMusic.Volume, t / fadeDuration);
                yield return null;
            }

            _crossfadeCoroutine = null;
        }

        public virtual void StopSFX(AudioSource source)
        {
            if (source == null) return;
            source.Stop();
            source.gameObject.SetActive(false);
            _sfxPool.Deactivate(source);
        }

        protected virtual IEnumerator PlayDelay(AudioSource source, float delay)
        {
            if (source == null) yield break;
            if (delay > 0f) yield return new WaitForSeconds(delay);

            source.Play();
        }
        protected virtual void Config(AudioSource source, SO_AudioParameters parameters)
        {
            if (source == null) return;
            if (parameters == null) return;

            source.gameObject.name = parameters.Name;
            source.clip = parameters.Clip;

            source.loop = parameters.Loop;
            source.playOnAwake = parameters.PlayOnAwake;

            source.volume = parameters.Volume;
            source.pitch = parameters.Pitch;

            source.spatialBlend = parameters.SpatialBlend;
            source.rolloffMode = parameters.VolumeRolloff;
            source.maxDistance = parameters.MaxDistance;
            source.minDistance = parameters.MinDistance;
        }

        protected void AutoDeactive(AudioSource source, Pool<AudioSource> pool, float? delay = null)
            => StartCoroutine(this.ReturnToPool(source, pool, delay));

        protected IEnumerator ReturnToPool(AudioSource source, Pool<AudioSource> pool, float? delay = null)
        {
            if (source == null) yield break;
            if (source.clip == null) yield break;

            float delayTime = delay ?? source.clip.length;
            yield return new WaitForSeconds(delayTime);

            source.Stop();
            source.gameObject.SetActive(false);
            if (pool != null) pool.Deactivate(source);
        }
    }
}
