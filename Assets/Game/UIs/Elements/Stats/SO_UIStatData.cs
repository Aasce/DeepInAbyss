using Asce.Game.Stats;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.UIs.Stats 
{
    [CreateAssetMenu(menuName = "Asce/UIs/Stat Data", fileName = "Stat Data")]
    public class SO_UIStatData : ScriptableObject
    {
        [SerializeField] private List<StatIconContainer> _icons = new()
        {
            new StatIconContainer(StatType.Health),
            new StatIconContainer(StatType.HealthScale),

            new StatIconContainer(StatType.Stamina),
            new StatIconContainer(StatType.Strength),
            new StatIconContainer(StatType.Armor),
            new StatIconContainer(StatType.Resistance),
            new StatIconContainer(StatType.Shield),

            new StatIconContainer(StatType.Hunger),
            new StatIconContainer(StatType.Thirst),
            new StatIconContainer(StatType.Breath),

            new StatIconContainer(StatType.Speed),
            new StatIconContainer(StatType.JumpForce),
            new StatIconContainer(StatType.ViewRadius),
        };
        private Dictionary<StatType, StatIconContainer> _iconDictionary = null;

        [Space]
        [SerializeField] private Color _healthBarCharacterColor;
        [SerializeField] private Color _healthBarNPCColor;
        [SerializeField] private Color _healthBarEnemyColor;


        public List<StatIconContainer> Data => _icons;
        public Dictionary<StatType, StatIconContainer> IconDictionary => _iconDictionary ?? this.CreateIconDictionary();

        public Color HealthBarCharacterColor => _healthBarCharacterColor;
        public Color HealthBarNPCColor => _healthBarNPCColor;
        public Color HealthBarEnemyColor => _healthBarEnemyColor;


        private Dictionary<StatType, StatIconContainer> CreateIconDictionary()
        {
            if (_iconDictionary == null) _iconDictionary = new();
            else _iconDictionary.Clear();

            foreach (StatIconContainer item in _icons)
            {
                if (_iconDictionary.ContainsKey(item.Type)) 
                    continue;
                
                _iconDictionary[item.Type] = item;
            }

            return _iconDictionary;
        }


        public StatIconContainer GetStatIcon(StatType type)
        {
            if (!IconDictionary.ContainsKey(type)) return null;
            return IconDictionary[type];
        }
    }

    [Serializable]
    public class StatIconContainer
    {
        [SerializeField, HideInInspector] private string _name;
        [SerializeField] private StatType _type = StatType.None;

        [Space]
        [SerializeField] private Sprite _icon = null;
        [SerializeField, ColorUsage(showAlpha: true)] private Color _color = Color.white;


        public StatType Type => _type;
        public Sprite Icon => _icon;
        public Color Color => _color;


        public StatIconContainer(StatType type = StatType.None) : this (type, null, Color.white) { }
        public StatIconContainer(StatType type, Sprite icon) : this (type, icon, Color.white) { }
        public StatIconContainer(StatType type, Color color) : this (type, null, color) { }
        public StatIconContainer(StatType type, Sprite icon, Color color) 
        {
            _name = type.ToString();
            _type = type;
            _icon = icon;
            _color = color;
        }
    }
}