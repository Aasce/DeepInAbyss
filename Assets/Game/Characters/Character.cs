using UnityEngine;

namespace Asce.Game.Entities
{
    public class Character : Creature, IControllableCharacter, IHasMovement<CharacterMovement>, IHasStats<CharacterStats, SO_CharacterBaseStats>
    {
        public new CharacterPhysicController PhysicController => base.PhysicController as CharacterPhysicController;
        public new CharacterMovement Movement => base.Movement as CharacterMovement;
        public new CharacterStats Stats => base.Stats as CharacterStats;


        protected override void Awake()
        {
            base.Awake();
            if (Movement != null) Movement.Owner = this;
        }


        protected virtual void Start()
        {
            Movement?.ResetJump();
        }
    }
}