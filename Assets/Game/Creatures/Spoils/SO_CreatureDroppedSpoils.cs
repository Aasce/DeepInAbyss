using Asce.Game.Items;
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
}