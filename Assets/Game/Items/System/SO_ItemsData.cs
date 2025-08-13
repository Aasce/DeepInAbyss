using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Asce.Game.Items
{
    [CreateAssetMenu(menuName = "Asce/Items/Items Data", fileName = "Items Data")]
    public class SO_ItemsData : ScriptableObject
    {
        [SerializeField] protected List<ItemContainer> _data = new();
        protected Dictionary<string, ItemContainer> _dataDictionary;

        public List<ItemContainer> Data => _data;
        public Dictionary<string, ItemContainer> DataDictionary => _dataDictionary ?? InitDictionary();

        protected virtual void Reset()
        {
            UpdateData();
        }

        protected virtual void OnEnable()
        {
            UpdateData();
        }

        protected virtual void OnValidate()
        {
            UpdateData();
        }

        public virtual SO_ItemInformation GetItemByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (!DataDictionary.TryGetValue(name, out ItemContainer container)) return null;
            return container.Information;
        }

        public virtual void AddItem(SO_ItemInformation information)
        {
            if (information == null) return;

            if (_data.Exists(c => c != null && c.Information == information || c.Name == information.Name))
            {
                Debug.LogWarning($"Item '{information.Name}' already exists.", information);
                return;
            }

            ItemContainer container = new (information);
            _data.Add(container);
            _dataDictionary?.Add(container.Name, container);
        }

        protected virtual Dictionary<string, ItemContainer> InitDictionary()
        {
            _dataDictionary ??= new();
            _dataDictionary.Clear();

            foreach (ItemContainer container in _data)
            {
                if (container == null || container.Information == null) continue;
                if (string.IsNullOrEmpty(container.Information.Name))
                {
                    Debug.LogWarning($"Unnamed item in ItemsData", container.Information);
                    continue;
                }

                if (_dataDictionary.ContainsKey(container.Name))
                {
                    Debug.LogWarning($"Duplicate item key '{container.Name}' detected.", container.Information);
                    continue;
                }

                _dataDictionary[container.Name] = container;
            }

            return _dataDictionary;
        }

        public virtual void UpdateData()
        {
            for (int i = _data.Count - 1; i >= 0; i--)
            {
                if (_data[i] == null || _data[i].Information == null)
                {
                    _data.RemoveAt(i);
                    continue;
                }

                _data[i].Reset();
            }

            InitDictionary();
        }
    }

    [Serializable]
    public class ItemContainer
    {
        [SerializeField, HideInInspector] private string _name;
        [SerializeField] private SO_ItemInformation _information;

        public string Name => _name;
        public SO_ItemInformation Information
        {
            get => _information;
            protected set => _information = value;
        }

        public ItemContainer(SO_ItemInformation information)
        {
            Information = information;
            this.Reset();
        }

        public void Reset()
        {
            if (_information == null) return;
            _name = _information.Name;
        }
    }
}