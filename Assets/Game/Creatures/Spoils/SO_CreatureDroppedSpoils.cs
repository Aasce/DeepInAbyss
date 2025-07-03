using Asce.Game.Items;
using Asce.Managers.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Entities
{
    [CreateAssetMenu(menuName = "Asce/Entities/Creature Dropped Spoils", fileName = "Creature Dropped Spoils")]
    public class SO_CreatureDroppedSpoils : ScriptableObject
    {
        [SerializeField] List<DroppedSpoilsContainer> _droppedSpoils = new();

        public List<DroppedSpoilsContainer> DroppedSpoils => _droppedSpoils;
    }

    [System.Serializable]
    public class DroppedSpoilsContainer
    {
        [SerializeField] protected SO_ItemInformation _item;
        [SerializeField, Range(0f, 1f)] protected float _dropChance = 1f;
        [SerializeField] protected Vector2Int _quantityRange = new (1, 1);


        public SO_ItemInformation Item => _item;
        public float DropChance => _dropChance;
        public Vector2Int quantityRange => _quantityRange;


        public DroppedSpoilsContainer() 
        { 
            
        }
    }
}