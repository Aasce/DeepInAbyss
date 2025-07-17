using Asce.Game.Enviroments;
using Asce.Game.Players;
using Asce.Managers;
using Asce.Managers.Utils;
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
            this.LoadAllBillboards();
            this.LoadAllChests();
        }


        public void SaveAll()
        {
            SaveLoadSystem.Save(new CharacterData(Player.Instance.MainCharacter), "player/character.json");
            this.SaveAllBillboards();
            this.SaveAllChests();
        }

        private void LoadMainCharacter()
        {
            CharacterData characterData = SaveLoadSystem.Load<CharacterData>("player/character.json");
            characterData?.Load(Player.Instance.MainCharacter);
            Player.Instance.CameraController.ToTarget(Vector2.up * 10f);
        }

        private void SaveAllBillboards()
        {
            AllBillboardData data = new();
            List<Billboard> billboards = ComponentUtils.FindAllComponentsInScene<Billboard>();

            foreach (Billboard billboard in billboards)
            {
                BillboardData billboardData = new();
                billboardData.Save(billboard);
                data.billboards.Add(billboardData);
            }

            SaveLoadSystem.Save(data, "scene/enviroments/billboards.json");
        }
        
        private void LoadAllBillboards()
        {
            AllBillboardData data = SaveLoadSystem.Load<AllBillboardData>("scene/enviroments/billboards.json");
            if (data == null) return;

            List<Billboard> billboards = ComponentUtils.FindAllComponentsInScene<Billboard>();

            foreach (Billboard billboard in billboards)
            {
                BillboardData match = data.billboards.Find(x => x.id ==  billboard.ID);
                match?.Load( billboard);
            }
        }

        private void SaveAllChests()
        {
            AllChestData data = new ();
            List<Chest> chests = ComponentUtils.FindAllComponentsInScene<Chest>();

            foreach (Chest chest in chests)
            {
                ChestData chestData = new();
                chestData.Save(chest);
                data.chests.Add(chestData);
            }

            SaveLoadSystem.Save(data, "scene/enviroments/chests.json");
        }

        private void LoadAllChests()
        {
            var data = SaveLoadSystem.Load<AllChestData>("scene/enviroments/chests.json");
            if (data == null) return;

            List<Chest> chests = ComponentUtils.FindAllComponentsInScene<Chest>();

            foreach (Chest chest in chests)
            {
                ChestData match = data.chests.Find(x => x.id == chest.ID);
                match?.Load(chest);
            }
        }

    }
}