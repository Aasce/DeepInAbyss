using Asce.Game.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Equipments
{
    public class WeaponView : MonoBehaviour, IView
    {
        [SerializeField] protected Weapon _weapon;

        [Space]
        [SerializeField] protected float _alpha = 1.0f;

        [SerializeField] protected Renderer _renderer;


        protected List<Renderer> _renderers = new();
        protected MaterialPropertyBlock _mpbAlpha;

        public virtual List<Renderer> Renderers => _renderers;
        public virtual MaterialPropertyBlock MPBAlpha => _mpbAlpha != null ? _mpbAlpha : _mpbAlpha = new MaterialPropertyBlock();


        public Weapon Weapon
        {
            get => _weapon;
            set => _weapon = value;
        }

        public virtual float Alpha
        {
            get => _alpha;
            set
            {
                _alpha = Mathf.Clamp01(value);
                this.SetRendererAlpha(_alpha);
            }
        }

        protected virtual void Awake()
        {
            this.ResetRendererList();
        }

        protected virtual void Start()
        {
            this.SetRendererAlpha(_alpha);
        }

        protected virtual void ResetRendererList()
        {
            Renderers.Clear();
            if (_renderer != null) Renderers.Add(_renderer);
        }

    }
}