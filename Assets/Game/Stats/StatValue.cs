using System;
using UnityEngine;

namespace Asce.Game.Stats
{
    [Serializable]
    public struct StatValue
    {
        [SerializeField] private StatType _type;
        [SerializeField] private float _value;
        [SerializeField] private StatValueType _valueType;

        public StatType Type
        {
            get => _type;
            set => _type = value;
        }

        public float Value
        {
            readonly get => _value;
            set => _value = value;
        }

        public StatValueType ValueType
        {
            readonly get => _valueType;
            set => _valueType = value;
        }

        public StatValue(StatType type, float value, StatValueType valueType)
        {
            _type = type;
            _value = value;
            _valueType = valueType;
        }
    }
}