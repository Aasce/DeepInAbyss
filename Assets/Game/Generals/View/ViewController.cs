using Asce.Game.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game
{
    public class ViewController : MonoBehaviour, IViewController
    {
        [Space]
        [SerializeField] protected float _alpha = 1.0f;

        protected List<Renderer> _renderers = new();
        protected MaterialPropertyBlock _mpbAlpha;

        public virtual List<Renderer> Renderers => _renderers;
        public virtual MaterialPropertyBlock MPBAlpha => _mpbAlpha != null ? _mpbAlpha : _mpbAlpha = new MaterialPropertyBlock();

        public virtual float Alpha
        {
            get => _alpha;
            set
            {
                _alpha = Mathf.Clamp01(value);
                this.SetRendererAlpha(_alpha);
            }
        }

        public virtual string SortingLayer
        {
            get => (Renderers.Count <= 0 || Renderers[0] == null) ? string.Empty : Renderers[0].sortingLayerName;
            set
            {
                foreach (Renderer renderer in Renderers)
                {
                    if (renderer == null) continue;
                    renderer.sortingLayerName = value;
                }
            }
        }

        public virtual int SortingOrder
        {
            get => (Renderers.Count <= 0 || Renderers[0] == null) ? 0 : Renderers[0].sortingOrder;
            set
            {
                foreach (Renderer renderer in Renderers)
                {
                    if (renderer == null) continue;
                    renderer.sortingOrder = value;
                }
            }
        }

        protected virtual void Reset()
        {

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
        }
    }
}