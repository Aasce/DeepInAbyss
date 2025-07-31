using Asce.Game.Items;
using Asce.Managers.Attributes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Asce.Game.Quests
{
    [CreateAssetMenu(menuName = "Asce/Quests/Quest Information", fileName = "Quest Information")]
    public class SO_QuestInformation : ScriptableObject
    {
        [SerializeField] protected string _name;
        [SerializeField, TextArea] protected string _description;
        [SerializeField, SpritePreview] protected Sprite _icon;

        [Space]
        [SerializeField] protected List<SO_QuestCondition> _conditions = new();

        [Space]
        [SerializeField] protected SO_DroppedSpoils _spoils;

        protected ReadOnlyCollection<SO_QuestCondition> _readonlyCondition;

        public string Name => _name;
        public string Description => _description;
        public Sprite Icon => _icon;

        public ReadOnlyCollection<SO_QuestCondition> Conditions => _readonlyCondition ??= _conditions.AsReadOnly();
        public SO_DroppedSpoils DroppedSpoils => _spoils;
    }
}
