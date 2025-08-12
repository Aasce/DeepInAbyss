using Asce.Game.Quests;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Enviroments
{
    [System.Serializable]
    public class Notice
    {
        [SerializeField] protected Quest _quest;

        public Quest Quest => _quest;

        public void SetQuest(Quest quest)
        {
            _quest = quest;
        }
    }
}