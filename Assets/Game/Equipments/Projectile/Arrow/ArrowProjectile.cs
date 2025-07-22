using Asce.Game.Entities;
using UnityEngine;

namespace Asce.Game.Equipments
{
    public class ArrowProjectile : Projectile
    {
        [Header("Arrow")]
        [SerializeField] protected float _insertMaxAngle = 60.0f; 
        [SerializeField] protected float _insertDepth = 0.1f;

        private Vector2 _hitVel;

        private bool _isAttachedToTarget;
        private float _curInsertDepth;


        protected override void Update()
        {
            if (!IsLaunched) return;

            base.Update();

            if (!HasHit)
            {
                float degAngle = Mathf.Atan2(_rigidbody.linearVelocityY, _rigidbody.linearVelocityX) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(degAngle, Vector3.forward);
            }

            if (_isAttachedToTarget)
            {
                if (_curInsertDepth < _insertDepth)
                {
                    transform.Translate(_hitVel * Time.deltaTime, Space.World);
                    _curInsertDepth += _hitVel.magnitude * Time.deltaTime;
                }
            }
        }

        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            if (HasHit) return;
            if (Owner != null && Owner.gameObject == collision.gameObject) return;

            if (Vector2.Angle(collision.contacts[0].normal, -transform.right) < _insertMaxAngle)
            {
                _isAttachedToTarget = true;

                if (collision.gameObject.TryGetComponent(out ICreature creature))
                {
                    Vector2 position = collision.contactCount > 0 ? collision.GetContact(0).point : (Vector2)collision.transform.position;
                    this.DealDamageTo(creature, position);
                }

                this.SetSortingLayer(collision);
                transform.SetParent(collision.collider.transform, true);

                _hitVel = -collision.relativeVelocity;
                _rigidbody.simulated = false;
                _collider.isTrigger = true;

                _despawnCooldown.CurrentTime = 5f;
            }

            base.OnCollisionEnter2D(collision);
        }

        private void SetSortingLayer(Collision2D collision)
        {
            IViewController viewController = collision.transform.GetComponentInChildren<IViewController>();
            if (viewController != null)
            {
                View.SortingLayer = viewController.SortingLayer;
                View.SortingOrder = viewController.SortingOrder - 1;
                return;
            }

            Renderer renderer = collision.transform.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                View.SortingLayer = renderer.sortingLayerName;
                View.SortingOrder = renderer.sortingOrder - 1;
                return;
            }
        }
    }
}
