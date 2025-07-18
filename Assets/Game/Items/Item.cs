using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Items
{
    /// <summary>
    ///     Represents a runtime instance of an item in the game,
    ///     with its base information and dynamically generated property data.
    /// </summary>
    [Serializable]
    public class Item
    {
        [SerializeField] protected SO_ItemInformation _information;

        // Stores runtime property data based on item type flags
        protected Dictionary<ItemPropertyType, ItemPropertyData> _propertiesData = new();

        /// <summary> Gets the base information ScriptableObject of this item. </summary>
        public SO_ItemInformation Information => _information;

        /// <summary>
        ///     Constructs an item from item information and loads its properties.
        /// </summary>
        /// <param name="information"> The base item data to use. </param>
        public Item(SO_ItemInformation information)
        {
            _information = information;
            this.LoadProperties();
        }

        /// <summary>
        ///     Gets a typed property data from the item.
        /// </summary>
        /// <typeparam name="T"> The type of ItemPropertyData to retrieve. </typeparam>
        /// <param name="type"> The property type enum. </param>
        /// <returns>
        ///     The property data cast to the requested type, or null if not found or not of that type.
        /// </returns>
        public virtual T GetProperty<T>(ItemPropertyType type) where T : ItemPropertyData
        {
            if (Information == null) return null;
            if (!_propertiesData.ContainsKey(type)) return null;

            return _propertiesData[type] as T;
        }

        /// <summary>
        ///     Gets the property data from the internal dictionary.
        /// </summary>
        /// <param name="type"> The type of property to retrieve. </param>
        /// <returns> The property data, or null if not found. </returns>
        public virtual ItemPropertyData GetProperty(ItemPropertyType type)
        {
            return this.GetProperty<ItemPropertyData>(type);
        }

        /// <summary>
        ///     Loads all property data associated with this item based on its defined flags.
        /// </summary>
        protected virtual void LoadProperties()
        {
            if (Information == null) return;

            _propertiesData.Clear();

            // For each property flag, create and store the runtime data
            foreach (ItemPropertyType propertyType in Information.PropertyType.GetFlags())
            {
                ItemPropertyData data = Information.CreatePropertyDataFromType(propertyType);
                if (data == null) continue;

                _propertiesData[propertyType] = data;
            }
        }
    }
}
