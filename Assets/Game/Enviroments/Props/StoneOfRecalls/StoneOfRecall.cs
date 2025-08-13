using Asce.Game.Spawners;
using Asce.Managers.Attributes;
using Asce.Managers.SaveLoads;
using Asce.Managers.Utils;
using Asce.Manager.Sounds;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Asce.Game.Enviroments
{
    public class StoneOfRecall : InteractiveObject, IUniqueIdentifiable, IEnviromentComponent, IInteractableObject, ISavePoint
    {
        [SerializeField, Readonly] protected string _id;

        // Ref
        [SerializeField, Readonly] protected BoxCollider2D _collider;
        [SerializeField] protected SpriteRenderer _glowRenderer;
        [SerializeField] protected VFXs.ParticleVFXObject _activeVFXPrefab;

        [Header("Stone of Recall")]
        [SerializeField] protected bool _isActive = false;
        [SerializeField] protected bool _isDefaultActive = false;
        protected bool _isLoaded = false;

        public event Action<object> OnActivate;

        public string ID => _id;
        public BoxCollider2D Collider => _collider;
        public SpriteRenderer GlowRenderer => _glowRenderer;

        public Vector2 Position => (Vector2)transform.position + Vector2.up * 0.5f;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive == value) return;
                _isActive = value;
                if (GlowRenderer != null) GlowRenderer.gameObject.SetActive(_isActive);
                if (_isActive)
                {
                    AudioManager.Instance.PlaySFX("Stone Of Recall Active", transform.position);
                    VFXs.VFXsManager.Instance.Spawn(_activeVFXPrefab, Position);
                    OnActivate?.Invoke(this);
                }
            }
        }

        protected override void RefReset()
        {
            base.RefReset();
            this.LoadComponent(out _collider);
        }

        protected virtual void Start()
        {
            _ = this.Load();
        }

        protected virtual async Task Load()
        {
            await SaveLoads.SaveLoadManager.Instance.WaitUntilLoadedAsync();
            if (_isLoaded) return;
            IsActive = _isDefaultActive;
            IsInteractable = !_isDefaultActive;
        }


        public override void Interact(GameObject interactor)
        {
            IsActive = true;
            IsInteractable = false;
            base.Interact(interactor);
        }

        public override void Focus()
        {
            base.Focus();

        }

        public override void Unfocus()
        {
            base.Unfocus();

        }

        void IReceiveData<bool>.Receive(bool isActive)
        {
            _isActive = isActive;
            if (GlowRenderer != null) GlowRenderer.gameObject.SetActive(_isActive);
            IsInteractable = !isActive;
            _isLoaded = true;
        }
    }
}
