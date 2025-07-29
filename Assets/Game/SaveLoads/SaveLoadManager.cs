using Asce.Game.Enviroments;
using Asce.Game.Enviroments.HiddenAreas;
using Asce.Game.Players;
using Asce.Game.Spawners;
using Asce.Managers;
using Asce.Managers.SaveLoads;
using Asce.Managers.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.SaveLoads
{
    public class SaveLoadManager : MonoBehaviourSingleton<SaveLoadManager>
    {
        private void Start()
        {
            this.LoadAll();
        }

        private void OnApplicationQuit()
        {
            this.SaveAll();
        }

        public void LoadAll()
        {
            this.LoadMainCharacter();
            LoadAllData<Chest, ChestData, AllChestData>(
                "scene/enviroments/chests.json",
                chest => data => data.id == chest.ID
            );

            LoadAllData<Billboard, BillboardData, AllBillboardData>(
                "scene/enviroments/billboards.json",
                billboard => data => data.id == billboard.ID
            );

            LoadAllData<ISavePoint, SavePointData, AllSavePointData>(
                "scene/enviroments/savepoints.json",
                savePoint => data => data.id == savePoint.ID
            );

            LoadAllData<HiddenArea, HiddenAreaData, AllHiddenAreaData>(
                "scene/enviroments/hidden_areas.json",
                hiddenArea => data => data.id == hiddenArea.ID
            );
        }


        public void SaveAll()
        {
            SaveLoadSystem.Save(new CharacterData(Player.Instance.MainCharacter), "player/character.json");
            SaveAllData<Chest, ChestData, AllChestData>("scene/enviroments/chests.json");
            SaveAllData<Billboard, BillboardData, AllBillboardData>("scene/enviroments/billboards.json");
            SaveAllData<ISavePoint, SavePointData, AllSavePointData>("scene/enviroments/savepoints.json");
            SaveAllData<HiddenArea, HiddenAreaData, AllHiddenAreaData>("scene/enviroments/hidden_areas.json");
            
        }

        private void LoadMainCharacter()
        {
            CharacterData characterData = SaveLoadSystem.Load<CharacterData>("player/character.json");
            characterData?.Load(Player.Instance.MainCharacter);
            Player.Instance.CameraController.ToTarget(Vector2.up * 10f);
        }


        /// <summary>
        ///     Saves all components of type <typeparamref name="TComponent"/> found in the scene to a file at the given path.
        /// </summary>
        /// <typeparam name="TComponent"> The component type to save. Must be a reference type. </typeparam>
        /// <typeparam name="TData"> The data container type that knows how to save a component of type <typeparamref name="TComponent"/>. </typeparam>
        /// <typeparam name="TGroupData"> The wrapper that holds a collection of <typeparamref name="TData"/> items. </typeparam>
        /// <param name="path"> The file path where the data will be saved. </param>
        private void SaveAllData<TComponent, TData, TGroupData>(string path)
            where TComponent : class
            where TData : ISaveData<TComponent>, new()
            where TGroupData : IGroupData<TData>, new()
        {
            // Create a container for the group of data objects
            TGroupData groupData = new();

            // Find all components of the specified type in the active scene
            List<TComponent> components = ComponentUtils.FindAllComponentsInScene<TComponent>();

            // Serialize each component into its data representation
            foreach (TComponent component in components)
            {
                TData data = new();
                data.Save(component); // Copy the component's state into the data object
                groupData.Items.Add(data); // Add to the group container
            }

            // Save the group data to the specified file path
            SaveLoadSystem.Save(groupData, path);
        }

        /// <summary>
        ///     Loads data from a file and applies it to components of type <typeparamref name="TComponent"/> in the scene.
        /// </summary>
        /// <typeparam name="TComponent"> The component type to apply the loaded data to. </typeparam>
        /// <typeparam name="TData"> The data container type that knows how to load into a component of type <typeparamref name="TComponent"/>. </typeparam>
        /// <typeparam name="TGroupData"> The wrapper that holds a collection of <typeparamref name="TData"/> items. </typeparam>
        /// <param name="path"> The file path from which the data will be loaded. </param>
        /// <param name="matchPredicateFactory">
        ///     A function that, given a component, returns a predicate to find the matching data object from the loaded list.
        /// </param>
        private void LoadAllData<TComponent, TData, TGroupData>(
            string path,
            Func<TComponent, Predicate<TData>> matchPredicateFactory)
            where TComponent : class
            where TData : ILoadData<TComponent>
            where TGroupData : IGroupData<TData>
        {
            // Load the group data from the specified file path
            TGroupData groupData = SaveLoadSystem.Load<TGroupData>(path);
            if (groupData == null) return;

            // Find all components of the specified type in the active scene
            List<TComponent> components = ComponentUtils.FindAllComponentsInScene<TComponent>();

            // Attempt to find and load matching data for each component
            foreach (TComponent component in components)
            {
                var predicate = matchPredicateFactory(component); // Create a predicate based on this component
                var match = groupData.Items.Find(predicate); // Find corresponding saved data
                match?.Load(component); // Load data into the component if match found
            }
        }

    }
}