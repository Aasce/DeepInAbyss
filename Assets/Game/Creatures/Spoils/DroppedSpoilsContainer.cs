using Asce.Game.Items;
using UnityEngine;

namespace Asce.Game.Entities
{
    [System.Serializable]
    public class DroppedSpoilsContainer
    {
        [SerializeField] protected SO_ItemInformation _item;
        [SerializeField, Range(0f, 1f)] protected float _dropChance = 1f;
        [SerializeField] protected Vector2Int _quantityRange = new(1, 1);
        [SerializeField] protected Vector2 _durationRatioRange = new(0, 1);


        public SO_ItemInformation ItemInformation => _item;
        public float DropChance => _dropChance;
        public Vector2Int QuantityRange => _quantityRange;
        public Vector2 DurationRatioRange => _durationRatioRange;


        public DroppedSpoilsContainer()
        {

        }
    }
}