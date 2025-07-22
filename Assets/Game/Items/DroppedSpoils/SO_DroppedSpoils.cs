using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Items
{
    [CreateAssetMenu(menuName = "Asce/Items/Dropped Spoils", fileName = "Dropped Spoils")]
    public class SO_DroppedSpoils : ScriptableObject
    {
        [SerializeField] protected List<DroppedSpoilsContainer> _droppedSpoils = new();

        public List<DroppedSpoilsContainer> DroppedSpoils => _droppedSpoils;
    }
}