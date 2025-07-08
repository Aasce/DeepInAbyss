using Asce.Game.VFXs;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Enviroments
{
    /// <summary>
    ///     Handles object interactions with a <see cref="Liquid"/> component.
    ///     <br/
    ///     Triggers effects and wave propagation when objects interact with the liquid.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class LiquidTriggerHandler : MonoBehaviour
    {
        /// <summary> The layer mask defining which layers can interact with the liquid. </summary>
        [SerializeField] protected LayerMask _interactiveLayer;
        [SerializeField] protected VFXObject _splashParticles;

        protected Liquid _liquid;


        public virtual Liquid Liquid => _liquid;

        protected virtual void Awake()
        {
            _liquid = GetComponent<Liquid>();
        }

        protected virtual void Start()
        {
            VFXsManager.Instance.Register(_splashParticles);
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (LayerUtils.IsInLayerMask(other.gameObject.layer, _interactiveLayer))
            {
                Rigidbody2D rb = other.GetComponentInParent<Rigidbody2D>();
                if (rb != null)
                {
                    this.SpawnParticles(other);

                    float velocity = CalculateVelocityToLiquid(rb.linearVelocityY);
                    Liquid.Splash(other.transform.position, other.bounds.extents.x, velocity);
                }
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            if (LayerUtils.IsInLayerMask(other.gameObject.layer, _interactiveLayer))
            {
                Rigidbody2D rb = other.GetComponentInParent<Rigidbody2D>();
                if (rb != null)
                {
                    this.SpawnParticles(other);

                    float velocity = CalculateVelocityToLiquid(rb.linearVelocityY);
                    Liquid.Splash(other.transform.position, other.bounds.extents.x, velocity);
                }
            }
        }

        protected virtual void SpawnParticles(Collider2D collision)
        {
            Vector2 hitObjectPosition = collision.transform.position;
            Bounds hitObjectBounds = collision.bounds;
            Bounds thisBounds = Liquid.Collider.bounds;

            Vector3 spawnPosition = Vector3.zero;
            if (hitObjectBounds.min.y > thisBounds.center.y)
            {
                // hit from above
                spawnPosition = hitObjectPosition - new Vector2(0f, hitObjectBounds.extents.y);
            }
            else
            {
                // hit from below
                spawnPosition = hitObjectPosition + new Vector2(0f, hitObjectBounds.extents.y);
            }

            if (_splashParticles != null) VFXsManager.Instance.Spawn(_splashParticles, spawnPosition, Quaternion.identity);
        }

        /// <summary>
        ///     Calculates the vertical impact velocity scaled by the liquid's force settings.
        /// </summary>
        /// <param name="vertiacalVelocity"> The vertical velocity of the rigidbody. </param>
        /// <returns> The adjusted and clamped velocity to apply to the splash system. </returns>
        protected virtual float CalculateVelocityToLiquid(float vertiacalVelocity)
        {
            int multiplier = (vertiacalVelocity < 0f) ? -1 : 1;

            float velocity = vertiacalVelocity * Liquid.ForceMultiplier;
            velocity = Mathf.Clamp(Mathf.Abs(velocity), 0f, Liquid.MaxForce);
            velocity *= multiplier;

            return velocity;
        }

    }
}