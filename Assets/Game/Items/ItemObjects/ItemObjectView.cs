using Asce.Game.Utils;
using Asce.Managers.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Items
{
    public class ItemObjectView : MonoBehaviour, IViewController
    {
        [SerializeField] protected SpriteRenderer _renderer;

        [Space]
        [SerializeField] protected float _alpha = 1.0f;

        protected List<Renderer> _renderers = new();
        protected MaterialPropertyBlock _mpbAlpha;

        public List<Renderer> Renderers => _renderers;

        public MaterialPropertyBlock MPBAlpha => _mpbAlpha != null ? _mpbAlpha : _mpbAlpha = new MaterialPropertyBlock();

        public virtual float Alpha
        {
            get => _alpha;
            set
            {
                _alpha = Mathf.Clamp01(value);
                this.SetRendererAlpha(_alpha);
            }
        }

        protected virtual void Reset()
        {
            this.LoadComponent(out _renderer);
        }

        protected virtual void Awake()
        {
            this.ResetRendererList();
        }

        protected virtual void Start()
        {
            this.SetRendererAlpha(_alpha);
        }

        public virtual void SetIcon(Sprite icon)
        {
            if (_renderer == null) return;

            _renderer.sprite = icon;
        }

        protected virtual void ResetRendererList()
        {
            Renderers.Clear();
            if (_renderer != null) Renderers.Add(_renderer);
        }
    }
}