using UnityEngine;

namespace Asce.Game.Entities
{
    public class Character : Creature, IControllableCharacter, IHasView<CharacterView>, IHasMovement<CharacterAction>, IHasStats<CharacterStats, SO_CharacterBaseStats>
    {
        public new CharacterPhysicController PhysicController => base.PhysicController as CharacterPhysicController;
        public new CharacterView View => base.View as CharacterView;
        public new CharacterAction Movement => base.Movement as CharacterAction;
        public new CharacterStats Stats => base.Stats as CharacterStats;


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