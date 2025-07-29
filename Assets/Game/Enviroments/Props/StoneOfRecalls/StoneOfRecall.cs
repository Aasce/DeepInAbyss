using Asce.Game.Spawners;
using Asce.Managers.Attributes;
using Asce.Managers.SaveLoads;
using Asce.Managers.Utils;
using System;
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
        [SerializeField] protected bool _isActive = true;

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
            IsActive = false;
        }

        public override void Interact(GameObject interactor)
        {
            IsActive = true;
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
        }
    }
}
