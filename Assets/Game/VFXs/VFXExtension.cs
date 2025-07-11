using Asce.Game.Entities;
using UnityEngine;

namespace Asce.Game.VFXs
{
    public static class VFXExtension
    {

        public static VFXObject RegisterAndSpawnEffect(this VFXsManager instace, string name, Vector3 position)
        {
            instace.StatusEffectVFXs.Register(name);
            return instace.Spawn(name, position);
        }

        public static void VFXFollowTarget(this VFXObject vfx, Transform target, Vector3 offset = default, bool isResetCooldown = true)
        {
            if (vfx == null) return;
            if (target == null) return;

            vfx.transform.position = target.position + offset;
            if (isResetCooldown) vfx.DespawnTime.Reset();
        }

        public static void VFXFollowCreature(this VFXObject vfx, Creature target, bool isResetCooldown = true)
        {
            if (vfx == null) return;
            if (target == null) return;

            float halfHeight = target.Status.Height * 0.5f;
            Vector3 offset = Vector3.up * halfHeight + Vector3.back;
            vfx.VFXFollowTarget(target.transform, offset, isResetCooldown);
        }
    }
}