using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Asce.Game.Quests
{
    [CreateAssetMenu(menuName = "Asce/Quests/Quests Data", fileName = "Quests Data")]
    public class SO_QuestsData : ScriptableObject
    {
        [SerializeField] protected List<SO_QuestInformation> _data = new();
        protected ReadOnlyCollection<SO_QuestInformation> _readonlyData;


        public ReadOnlyCollection<SO_QuestInformation> Data => _readonlyData ??= _data.AsReadOnly();

        public SO_QuestInformation GetQuestByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            foreach (SO_QuestInformation item in _data)
            {
                if (item ==  null) continue;
                if (item.Name == name) return item;
            }
            return null;
        }
    }
}
