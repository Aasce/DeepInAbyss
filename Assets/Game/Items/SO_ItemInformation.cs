using Asce.Managers.Attributes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Asce.Game.Items
{
    [CreateAssetMenu(menuName = "Asce/Items/Item Information", fileName = "Item Information")]
    public class SO_ItemInformation : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] protected string _name = string.Empty;
        [SerializeField, TextArea(3, 10)] protected string _description = string.Empty;
        [SerializeField, SpritePreview] protected Sprite _icon = null;

        [Space]
        [SerializeField] protected ItemType _type = ItemType.Unknown;
        [SerializeField] protected ItemRarityType _rarity = ItemRarityType.None;
        [SerializeField] protected ItemPropertyType _propertyType = ItemPropertyType.None;

        [Header("Properties")]
        [SerializeField, SerializeReference] protected List<ItemProperty> _properties = new();

        protected Dictionary<ItemPropertyType, ItemProperty> _propertyDictionary;
        protected ReadOnlyCollection<ItemProperty> _readonlyProperties;

        public string Name => _name;
        public string Description => _description;
        public Sprite Icon => _icon;

        public ItemType Type => _type;
        public ItemRarityType Rarity => _rarity;
        public ItemPropertyType PropertyType => _propertyType;

        public ReadOnlyCollection<ItemProperty> Properties => _readonlyProperties ??= _properties.AsReadOnly();

        public bool HasProperty(ItemPropertyType type)
        {
            if (_propertyDictionary == null) InitDictionary();
            return _propertyDictionary.ContainsKey(type);
        }
        public ItemProperty GetPropertyByType(ItemPropertyType type)
        {
            if (_propertyDictionary == null) this.InitDictionary();
            if (_propertyDictionary.TryGetValue(type, out ItemProperty property))
            {
                return property;
            }
            return null;
        }

        protected void InitDictionary()
        {
            _propertyDictionary = new();
            foreach (ItemProperty property in _properties)
            {
                if (property == null) continue;
                if (_propertyDictionary.ContainsKey(property.PropertyType)) continue;
                
                _propertyDictionary.Add(property.PropertyType, property);
            }
        }
    }
}