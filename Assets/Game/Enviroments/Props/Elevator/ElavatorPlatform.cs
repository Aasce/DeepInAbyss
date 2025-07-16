using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Enviroments
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class ElavatorPlatform : Platform, IEnviromentComponent
    {
        [SerializeField, Readonly] protected Rigidbody2D _rigidbody;
        [SerializeField, Readonly] protected BoxCollider2D _collider;

        [Space]
        [SerializeField] protected float _velocityInheritRatio = 0.8f;
        protected HashSet<Transform> _bodyOnPlatform = new();

        protected Vector3 _prePosition;
        protected Vector2 _velocity;

        public Rigidbody2D Rigidbody => _rigidbody;
        public BoxCollider2D Collider => _collider;

        protected override void RefReset()
        {
            base.RefReset();
            this.LoadComponent(out _rigidbody);
            this.LoadComponent(out _collider);
        }

        private void FixedUpdate()
        {
            _velocity = (transform.position - _prePosition) / Time.fixedDeltaTime;
            _prePosition = transform.position;

            foreach (Transform t in _bodyOnPlatform)
            {
                t.Translate(_velocity * Time.fixedDeltaTime);
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.attachedRigidbody == null) return;
            if (collision.attachedRigidbody.bodyType != RigidbodyType2D.Dynamic) return;

            _bodyOnPlatform.Add(collision.transform);
            collision.attachedRigidbody.linearVelocity -= _velocity * _velocityInheritRatio;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.attachedRigidbody == null) return;
            if (collision.attachedRigidbody.bodyType != RigidbodyType2D.Dynamic) return;
            if (!_bodyOnPlatform.Contains(collision.transform)) return;

            _bodyOnPlatform.Remove(collision.transform);
            collision.attachedRigidbody.linearVelocity += _velocity * _velocityInheritRatio;
        }
    }
}