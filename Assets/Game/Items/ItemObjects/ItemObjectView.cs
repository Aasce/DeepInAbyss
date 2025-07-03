using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Items
{
    public class ItemObjectView : ViewController
    {
        [SerializeField] protected Animator _animator;
        [SerializeField] protected SpriteRenderer _renderer;

        public Animator Animator => _animator;

        protected override void Reset()
        {
            base.Reset();
            this.LoadComponent(out _animator);
            this.LoadComponent(out _renderer);
        }

        public virtual void SetIcon(Sprite icon)
        {
            if (_renderer == null) return;

            _renderer.sprite = icon;
        }

        public virtual void IsBlinking(bool isBlinking)
        {
            if(Animator != null) Animator.SetBool("IsBlinking", isBlinking);
        }

        protected override void ResetRendererList()
        {
            base.ResetRendererList();
            if (_renderer != null) Renderers.Add(_renderer);
        }
    }
}