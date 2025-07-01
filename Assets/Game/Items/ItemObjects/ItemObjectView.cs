using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Items
{
    public class ItemObjectView : ViewController
    {
        [SerializeField] protected SpriteRenderer _renderer;


        protected override void Reset()
        {
            base.Reset();
            this.LoadComponent(out _renderer);
        }

        public virtual void SetIcon(Sprite icon)
        {
            if (_renderer == null) return;

            _renderer.sprite = icon;
        }

        protected override void ResetRendererList()
        {
            base.ResetRendererList();
            if (_renderer != null) Renderers.Add(_renderer);
        }
    }
}