using Asce.Game.StatusEffects;
using Asce.Managers.Pools;
using Asce.Managers.UIs;
using UnityEngine;

namespace Asce.Game.UIs.StatusEffects
{
    public class UIStatusEffectController : UIObject
    {
        [SerializeField] protected Pool<UIStatusEffect> _pool = new();
        protected StatusEffectController _controller;

        public virtual void SetStatusEffectController(StatusEffectController controller)
        {
            this.Unregister();
            _controller = controller;
            this.Register();
        }

        protected virtual void Register()
        {
            if (_controller == null) return;
            this.LoadEffects();
            _controller.OnEffectAdded += Controller_OnEffectAdded;
            _controller.OnEffectRemoved += Controller_OnEffectRemoved;
            _controller.OnEffectsCleared += Controller_OnEffectsCleared;
        }

        protected virtual void Unregister()
        {
            if (_controller == null) return;
            _controller.OnEffectAdded -= Controller_OnEffectAdded;
            _controller.OnEffectRemoved -= Controller_OnEffectRemoved;
            _controller.OnEffectsCleared -= Controller_OnEffectsCleared;
        }

        protected virtual void LoadEffects()
        {
            if ( _controller == null) return;

            _pool.Clear(isDeactive: true);
            foreach (StatusEffect effect in _controller.Effects)
            {
                if (effect.IsNull()) continue;

                UIStatusEffect uiEffect = _pool.Activate();
                if (uiEffect == null) continue;

                uiEffect.transform.SetAsLastSibling();
                uiEffect.name = effect.Name;
                uiEffect.SetStatusEffect(effect);
                uiEffect.Show();
            }
        }

        protected virtual void Controller_OnEffectAdded(object sender, StatusEffect effect)
        {
            UIStatusEffect uiEffect = _pool.Activate();
            if (uiEffect == null) return;

            uiEffect.transform.SetAsLastSibling();
            uiEffect.name = effect.Name;
            uiEffect.SetStatusEffect(effect);
            uiEffect.Show();
        }

        protected virtual void Controller_OnEffectRemoved(object sender, StatusEffect effect)
        {
            UIStatusEffect uiEffect = _pool.Activities.Find((ui) => ui.StatusEffect == effect);
            if (uiEffect == null) return;

            uiEffect.Hide();
            _pool.Deactivate(uiEffect);
        }

        protected virtual void Controller_OnEffectsCleared(object sender)
        {
            _pool.Clear(isDeactive: true);
        }

    }
}
