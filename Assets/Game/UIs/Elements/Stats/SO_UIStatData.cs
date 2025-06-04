using Asce.Game.Stats;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.UIs.Stats 
{
    [CreateAssetMenu(menuName = "Asce/UIs/Stat Icon Data", fileName = "Stat Icon Data")]
    public class SO_UIStatData : ScriptableObject
    {
        [SerializeField] private List<StatDataContainer> _data = new()
        {
            new StatDataContainer(StatType.Health),
            new StatDataContainer(StatType.HealthScale),

            new StatDataContainer(StatType.Stamina),
            new StatDataContainer(StatType.Strength),
            new StatDataContainer(StatType.Armor),
            new StatDataContainer(StatType.Resistance),
            new StatDataContainer(StatType.Shield),

            new StatDataContainer(StatType.Hunger),
            new StatDataContainer(StatType.Thirst),
            new StatDataContainer(StatType.Breath),

            new StatDataContainer(StatType.Speed),
            new StatDataContainer(StatType.JumpForce),
            new StatDataContainer(StatType.ViewRadius),
        };

        private Dictionary<StatType, StatDataContainer> _dataDictionary = null;

        public List<StatDataContainer> Data => _data;
        public Dictionary<StatType, StatDataContainer> DataDictionary => _dataDictionary ?? this.CreateDictionary();

        private Dictionary<StatType, StatDataContainer> CreateDictionary()
        {
            if (_dataDictionary == null) _dataDictionary = new();
            else _dataDictionary.Clear();

            foreach (StatDataContainer item in _data)
            {
                if (_dataDictionary.ContainsKey(item.Type)) 
                    continue;
                
                _dataDictionary[item.Type] = item;
            }

            return _dataDictionary;
        }


        public StatDataContainer GetStat(StatType type)
        {
            if (!DataDictionary.ContainsKey(type)) return null;
            return DataDictionary[type];
        }
    }

    [Serializable]
    public class StatDataContainer
    {
        [SerializeField, HideInInspector] private string _name;
        [SerializeField] private StatType _type = StatType.None;

        [Space]
        [SerializeField] private Sprite _icon = null;
        [SerializeField, ColorUsage(showAlpha: true)] private Color _color = Color.white;


        public StatType Type => _type;
        public Sprite Icon => _icon;
        public Color Color => _color;


        public StatDataContainer(StatType type = StatType.None) : this (type, null, Color.white) { }
        public StatDataContainer(StatType type, Sprite icon) : this (type, icon, Color.white) { }
        public StatDataContainer(StatType type, Color color) : this (type, null, color) { }
        public StatDataContainer(StatType type, Sprite icon, Color color) 
        {
            _name = type.ToString();
            _type = type;
            _icon = icon;
            _color = color;
        }
    }
}