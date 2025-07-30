using Asce.Managers.Attributes;
using UnityEngine;

namespace Asce.Game.VFXs
{
    [CreateAssetMenu(menuName = "Asce/VFXs/Full Screen VFX Parameters", fileName= "Full Screen VFX Parameters")]
    public class SO_FullScreenVFXParameters : ScriptableObject
    {
        [Header("Properties")]
        [SerializeField] protected string _name = "Full Screen VFX";
        [SerializeField, Min(0f)] protected float _duration = 1f;
        [SerializeField] protected FullScreenEffectType _effectType = FullScreenEffectType.FadeOut;

        [Header("Color")]
        [SerializeField, ColorUsage(showAlpha: true, hdr: true)] protected Color _color = Color.white;

        [Header("Texture")]
        [SerializeField] protected Texture _texture = null;
        [SerializeField] protected Vector2 _textureTiling = Vector2.one;
        [SerializeField] protected Vector2 _textureOffset = Vector2.zero;
        [SerializeField] protected Vector2 _textureScroll = Vector2.zero;

        [Header("Vignette")]
        [SerializeField, Min(0f)] protected float _vignettePower = 0.5f;
        [SerializeField, Min(0f)] protected float _vignetteIntensity = 1f;


        public string Name => _name;
        public float Duration => _duration;
        public FullScreenEffectType EffectType => _effectType;

        public Color Color => _color;

        public Texture Texture => _texture;
        public Vector2 TextureTiling => _textureTiling;
        public Vector2 TextureOffset => _textureOffset;
        public Vector2 TextureScroll => _textureScroll;

        public float VignettePower => _vignettePower;
        public float VignetteIntensity => _vignetteIntensity;
    }
}
