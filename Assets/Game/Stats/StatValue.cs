using System;
using UnityEngine;

namespace Asce.Game.Stats
{
    [Serializable]
    public struct StatValue
    {
        [SerializeField] private float _value;
        [SerializeField] private StatValueType _type;

        public float Value
        {
            readonly get => _value;
            set => _value = value;
        }

        public StatValueType Type
        {
            readonly get => _type;
            set => _type = value;
        }

        public StatValue(float value, StatValueType type)
        {
            _value = value;
            _type = type;
        }
    }
}