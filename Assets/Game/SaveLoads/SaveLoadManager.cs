using Asce.Game.Enviroments;
using Asce.Game.Players;
using Asce.Managers;
using Asce.Managers.Utils;
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
        }


        public void SaveAll()
        {
            SaveLoadSystem.Save(new CharacterData(Player.Instance.MainCharacter), "player/character.json");
            this.SaveAllBillboards();
        }

        private void LoadMainCharacter()
        {
            CharacterData characterData = SaveLoadSystem.Load<CharacterData>("player/character.json");
            characterData?.Load(Player.Instance.MainCharacter);
            Player.Instance.CameraController.ToTarget(Vector2.up * 10f);
        }
        private void SaveAllBillboards()
        {
            var data = new AllBillboardData();
            var billboards = ComponentUtils.FindAllComponentsInScene<Billboard>();

            foreach (var b in billboards)
            {
                var bData = new BillboardData();
                bData.Save(b);
                data.billboards.Add(bData);
            }

            SaveLoadSystem.Save(data, "scene/enviroments/billboards.json");
        }

        private void LoadAllBillboards()
        {
            var data = SaveLoadSystem.Load<AllBillboardData>("scene/enviroments/billboards.json");
            if (data == null) return;

            var billboards = ComponentUtils.FindAllComponentsInScene<Billboard>();

            foreach (var b in billboards)
            {
                var match = data.billboards.Find(x => x.id == b.ID);
                match?.Load(b);
            }
        }

    }
}