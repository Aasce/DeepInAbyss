using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities
{
    public class Character : Creature, IControllableCharacter, 
        IHasView<CharacterView>, IHasUI<CharacterUI>, IHasAction<CharacterAction>, IHasStats<CharacterStats, SO_CharacterBaseStats>, 
        IHasEquipment<CharacterEquipment>, IHasInventory<CharacterInventory>
    {
        public new CharacterPhysicController PhysicController => base.PhysicController as CharacterPhysicController;
        public new CharacterView View => base.View as CharacterView;
        public new CharacterUI UI => base.UI as CharacterUI;
        public new CharacterAction Action => base.Action as CharacterAction;
        public new CharacterStats Stats => base.Stats as CharacterStats;
        public new CharacterEquipment Equipment => base.Equipment as CharacterEquipment;
        public new CharacterInventory Inventory => base.Inventory as CharacterInventory;


        protected override void Reset()
        {
            base.Reset();
        }

        protected override void Awake()
        {
            base.Awake();
        }


        protected override void Start()
        {
            base.Start();
        }

    }
}