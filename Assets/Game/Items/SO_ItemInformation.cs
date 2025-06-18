using UnityEngine;

namespace Asce.Game.Items
{
    [CreateAssetMenu(menuName = "Asce/Items/Item Information", fileName = "Item Information")]
    public class SO_ItemInformation : ScriptableObject
    {
        [SerializeField] protected string _name = string.Empty;
        [SerializeField, TextArea] protected string _description = string.Empty;
        [SerializeField] protected Sprite _icon = null;

        [Space]
        [SerializeField] protected ItemRarityType _rarity = ItemRarityType.None;


        public string Name => _name;
        public string Description => _description;
        public Sprite Icon => _icon;
        public ItemRarityType Rarity => _rarity;
    }
}