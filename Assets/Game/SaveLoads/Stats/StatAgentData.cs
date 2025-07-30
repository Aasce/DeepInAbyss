using Asce.Game.Stats;
using Asce.Managers.SaveLoads;
using Asce.Managers.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class StatAgentData : SaveData, ISaveData<StatAgent>, ICreateData<StatAgent>
    {
        public string id;
        public string reason;
        public float value;
        public StatValueType valueType;


        public void Save(in StatAgent target)
        {
            if (target == null) return;
            if (target.Author != null && target.Author.TryGetComponent(out IUniqueIdentifiable identifiable))
            {
                id = identifiable.ID;
            }
            reason = target.Reason;
            value = target.Value;
            valueType = target.ValueType;
        }

        public StatAgent Create()
        {
            GameObject author = FindAuthor();
            StatAgent agent = new()
            {
                Author = author,
                Reason = reason,
                Value = value,
                ValueType = valueType
            };
            return agent;
        }

        public GameObject FindAuthor()
        {
            GameObject author = null;
            List<IUniqueIdentifiable> identifiables = ComponentUtils.FindAllComponentsInScene<IUniqueIdentifiable>();
            foreach (IUniqueIdentifiable identifiable in identifiables)
            {
                if (identifiable.ID == id)
                {
                    author = identifiable.gameObject;
                    break;
                }
            }
            return author;
        }
    }
}