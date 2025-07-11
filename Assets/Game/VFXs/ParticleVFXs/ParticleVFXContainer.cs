using UnityEngine;

namespace Asce.Game.VFXs
{
    [System.Serializable]
    public class ParticleVFXContainer
    {
        [SerializeField] protected ParticleSystem _vfx;
        [SerializeField] protected ParticleSystemStopBehavior _stopMode;

        public ParticleSystemStopBehavior StopMode
        {
            get => _stopMode;
            set => _stopMode = value;
        }

        public ParticleSystem VFX => _vfx;

        public ParticleVFXContainer() { }
        public ParticleVFXContainer(ParticleSystem vfx, ParticleSystemStopBehavior stopMode = ParticleSystemStopBehavior.StopEmittingAndClear) 
        {
            _vfx = vfx;
            _stopMode = stopMode;
        }
    }
}