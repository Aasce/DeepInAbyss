using Asce.Game.Items;
using Asce.Game.Players;
using Asce.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.SaveLoads
{
    public class SaveLoadManager : MonoBehaviourSingleton<SaveLoadManager>
    {

        protected override void Awake()
        {
            base.Awake();
            _ = Player.Instance;
        }

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
        }


        public void SaveAll()
        {
            CharacterData characterData = new(Player.Instance.MainCharacter);
            SaveLoadSystem.Save(characterData, "player/character.json");
        }

        private void LoadMainCharacter()
        {
            CharacterData characterData = SaveLoadSystem.Load<CharacterData>("player/character.json");
            characterData?.Load(Player.Instance.MainCharacter);
            Player.Instance.CameraController.ToTarget(Vector2.up * 10f);
        }
    }
}