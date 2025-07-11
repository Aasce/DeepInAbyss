using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.VFXs
{
    public class ParticleVFXObject : VFXObject
    {
        [SerializeField] protected List<ParticleVFXContainer> _particleSystems = new();

        protected virtual void Reset()
        {
            _particleSystems.Clear();
            ParticleSystem[] particleSystems = transform.GetComponentsInChildren<ParticleSystem>();
            foreach(ParticleSystem particleSystem in particleSystems)
            {
                if (particleSystem == null) continue;
                _particleSystems.Add(new(particleSystem));
            }
        }

        public override void Stop()
        {
            float maxLifetime = 0f;
            foreach (ParticleVFXContainer container in _particleSystems)
            {
                if (container == null) continue;
                if (container.VFX == null) continue;

                switch (container.StopMode)
                {
                    case ParticleSystemStopBehavior.StopEmittingAndClear:
                        container.VFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                        break;

                    case ParticleSystemStopBehavior.StopEmitting:
                        container.VFX.Stop(true, ParticleSystemStopBehavior.StopEmitting);

                        float lifetime = GetMaxParticleLifetime(container.VFX);
                        maxLifetime = Mathf.Max(maxLifetime, lifetime);
                        break;
                }
            }

            DespawnTime.CurrentTime = maxLifetime + 1f;
        }

        float GetMaxParticleLifetime(ParticleSystem particleSystem)
        {
            if (particleSystem == null) return 0f;

            var main = particleSystem.main;

            float startLifetime;
            if (main.startLifetime.mode == ParticleSystemCurveMode.TwoConstants)
                startLifetime = Mathf.Max(main.startLifetime.constantMin, main.startLifetime.constantMax);
            else
                startLifetime = main.startLifetime.constant;

            float maxLifetime = startLifetime;

            if (particleSystem.subEmitters.enabled)
            {
                for (int i = 0; i < particleSystem.subEmitters.subEmittersCount; i++)
                {
                    var sub = particleSystem.subEmitters.GetSubEmitterSystem(i);
                    maxLifetime = Mathf.Max(maxLifetime, GetMaxParticleLifetime(sub));
                }
            }

            return maxLifetime;
        }

    }
}