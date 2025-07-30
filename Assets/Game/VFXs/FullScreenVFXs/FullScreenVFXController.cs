using Asce.Managers;
using System.Collections;
using UnityEngine;

namespace Asce.Game.VFXs
{
    public class FullScreenVFXController : GameComponent
    {
        protected readonly string _colorProperty = "_Color";
        protected readonly string _strenghtProperty = "_Strength";
        protected readonly string _textureProperty = "_Texture";
        protected readonly string _textureTilingProperty = "_TextureTiling";
        protected readonly string _textureOffsetProperty = "_TextureOffset";
        protected readonly string _textureScrollProperty = "_TextureScroll";
        protected readonly string _vignettePowerProperty = "_VignettePower";
        protected readonly string _vignetteIntensityProperty = "_VignetteIntensity";

        [SerializeField] protected Material _fullScreenMaterial;
        [SerializeField] protected SO_FullScreenVFXData _fullScreenVFXData;

        protected Coroutine _effectCoroutine;

        protected virtual void Start()
        {
            if (_fullScreenMaterial == null) return;
            _fullScreenMaterial.SetFloat(_strenghtProperty, 0f);
        }

        public void Set(string name, float? duration = null)
        {
            if (_fullScreenMaterial == null) return;
            if (_fullScreenVFXData == null) return;
            if (string.IsNullOrEmpty(name)) return;

            SO_FullScreenVFXParameters parameters = _fullScreenVFXData.GetParameterByName(name);
            this.Set(parameters, duration);
        }

        public void Set(SO_FullScreenVFXParameters parameters, float? duration = null)
        {
            if (_fullScreenMaterial == null) return;
            if (parameters == null) return;

            _fullScreenMaterial.SetColor(_colorProperty, parameters.Color);
            _fullScreenMaterial.SetTexture(_textureProperty, parameters.Texture);
            _fullScreenMaterial.SetVector(_textureTilingProperty, parameters.TextureTiling);
            _fullScreenMaterial.SetVector(_textureOffsetProperty, parameters.TextureOffset);
            _fullScreenMaterial.SetVector(_textureScrollProperty, parameters.TextureScroll);
            _fullScreenMaterial.SetFloat(_vignettePowerProperty, parameters.VignettePower);
            _fullScreenMaterial.SetFloat(_vignetteIntensityProperty, parameters.VignetteIntensity);

            if (_effectCoroutine != null)
                StopCoroutine(_effectCoroutine);

            _effectCoroutine = StartCoroutine(EffectRoutine(parameters.EffectType, duration ?? parameters.Duration));
        }

        protected IEnumerator EffectRoutine(FullScreenEffectType effectType, float effectDuration)
        {
            switch (effectType)
            {
                case FullScreenEffectType.FadeOut:
                    yield return FadeOutRoutine(effectDuration);
                    break;

                case FullScreenEffectType.HoldThenHide:
                    yield return HoldThenHideRoutine(effectDuration);
                    break;

                case FullScreenEffectType.FadeInOut:
                    yield return FadeInOutRoutine(effectDuration);
                    break;

                case FullScreenEffectType.Pulse:
                    yield return PulseRoutine(effectDuration);
                    break;
            }

            _effectCoroutine = null;
        }

        protected IEnumerator FadeOutRoutine(float duration)
        {
            _fullScreenMaterial.SetFloat(_strenghtProperty, 1f);

            float timer = duration;
            while (timer >= 0f)
            {
                float t = timer / duration;
                _fullScreenMaterial.SetFloat(_strenghtProperty, t);
                timer -= Time.deltaTime;
                yield return null;
            }

            _fullScreenMaterial.SetFloat(_strenghtProperty, 0f);
        }

        protected IEnumerator HoldThenHideRoutine(float duration)
        {
            _fullScreenMaterial.SetFloat(_strenghtProperty, 1f);
            yield return new WaitForSeconds(duration);
            _fullScreenMaterial.SetFloat(_strenghtProperty, 0f);
        }

        protected IEnumerator FadeInOutRoutine(float duration)
        {
            float halfDuration = duration / 2f;
            float timer = 0f;

            // Fade in
            while (timer < halfDuration)
            {
                float t = timer / halfDuration;
                _fullScreenMaterial.SetFloat(_strenghtProperty, t);
                timer += Time.deltaTime;
                yield return null;
            }

            // Fade out
            timer = halfDuration;
            while (timer >= 0f)
            {
                float t = timer / halfDuration;
                _fullScreenMaterial.SetFloat(_strenghtProperty, t);
                timer -= Time.deltaTime;
                yield return null;
            }

            _fullScreenMaterial.SetFloat(_strenghtProperty, 0f);
        }

        protected IEnumerator PulseRoutine(float duration)
        {
            float timer = 0f;
            float pulseSpeed = 4f; // oscillations per second (can be made parameterized later)

            while (timer < duration)
            {
                float t = Mathf.Sin(timer * pulseSpeed * Mathf.PI * 2f) * 0.5f + 0.5f; // oscillates between 0 and 1
                _fullScreenMaterial.SetFloat(_strenghtProperty, t);

                timer += Time.deltaTime;
                yield return null;
            }

            _fullScreenMaterial.SetFloat(_strenghtProperty, 0f);
        }
    }
}
