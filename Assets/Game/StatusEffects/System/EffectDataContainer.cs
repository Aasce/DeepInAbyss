using System;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    [Serializable]
    public class EffectDataContainer
    {
        [SerializeField] protected int _level = 1;
        [SerializeField] protected float _strength = 0f;
        [SerializeField] protected float _duration = 1f;
        [SerializeField] protected int _maxStack = 1;


        public int Level
        {
            get => _level;
            set => _level = value;
        }

        public float Strength
        {
            get => _strength;
            set => _strength = value;
        }

        public float Duration
        {
            get => _duration;
            set => _duration = value;
        }

        public int MaxStack
        {
            get => _maxStack;
            set => _maxStack = value;
        }


        public EffectDataContainer() { }

        public EffectDataContainer Clone()
        {
            return new EffectDataContainer()
            {
                Level = Level,
                Strength = Strength,
                Duration = Duration,
                MaxStack = MaxStack,
            };
        }

    }
}