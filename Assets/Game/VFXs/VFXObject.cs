using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.VFXs
{
    /// <summary>
    ///     Represents a visual effect (VFX) object that has a despawn timer.
    ///     <br/>
    ///     Used in conjunction with a VFX pooling system to control activation duration.
    /// </summary>
    public class VFXObject : MonoBehaviour
    {
        [Tooltip("The cooldown timer that tracks how long the VFX should stay active before being despawned.")]
        [SerializeField] protected Cooldown _despawnTime = new();

        /// <summary>
        ///     Gets the cooldown timer used to determine when the VFX should be deactivated.
        /// </summary>
        public Cooldown DespawnTime => _despawnTime;
    }
}
