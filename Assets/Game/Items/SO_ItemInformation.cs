using Asce.Managers.Attributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Asce.Game.Items
{
    [CreateAssetMenu(menuName = "Asce/Items/Item Information", fileName = "Item Information")]
    public class SO_ItemInformation : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] protected string _name = string.Empty;
        [SerializeField, TextArea] protected string _description = string.Empty;
        [SerializeField, SpritePreview] protected Sprite _icon = null;

        [Space]
        [SerializeField] protected ItemType _type = ItemType.Unknown;
        [SerializeField] protected ItemRarityType _rarity = ItemRarityType.None;
        [SerializeField] protected ItemPropertyType _propertyType = ItemPropertyType.None;

        [Header("Properties")]
        [SerializeField, SerializeReference] private List<ItemProperty> _properties = new();

        public string Name => _name;
        public string Description => _description;
        public Sprite Icon => _icon;

        public ItemType Type => _type;
        public ItemRarityType Rarity => _rarity;
        public ItemPropertyType PropertyType => _propertyType;

        public T GetProperty<T>() where T : ItemProperty => _properties.OfType<T>().FirstOrDefault();
        public bool HasProperty(ItemPropertyType type) => _propertyType.HasFlag(type);
    }
}