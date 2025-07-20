using Asce.Game.Combats;
using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Entities.Characters
{
    public class Character : Creature, IControllableCharacter, 
        IHasView<CharacterView>, IHasUI<CharacterUI>, IHasAction<CharacterAction>, IHasInteraction<CharacterInteraction>,
        IHasStats<CharacterStats, SO_CharacterBaseStats>, 
        IHasEquipment<CharacterEquipment>, IHasInventory<CharacterInventory>
    {
        [SerializeField, Readonly] protected CharacterInteraction _interaction;

        [Space]
        [SerializeField] protected HitBox _damageHitBox = new();
        protected HashSet<GameObject> _damagedObject = new();


        public new CharacterPhysicController PhysicController => base.PhysicController as CharacterPhysicController;
        public new CharacterView View => base.View as CharacterView;
        public new CharacterUI UI => base.UI as CharacterUI;
        public new CharacterAction Action => base.Action as CharacterAction;
        public new CharacterStats Stats => base.Stats as CharacterStats;
        public new CharacterEquipment Equipment => base.Equipment as CharacterEquipment;
        public new CharacterInventory Inventory => base.Inventory as CharacterInventory;
        
        public CharacterInteraction Interaction => _interaction;

        public HitBox DamageHitBox => _damageHitBox;


        protected override void Reset()
        {
            base.Reset();
        }

        protected override void RefReset()
        {
            base.RefReset();
            if (this.LoadComponent(out _interaction)) 
            {
                Interaction.Owner = this;
            }
        }

        protected override void Awake()
        {
            base.Awake();
        }


        protected override void Start()
        {
            base.Start();
            Action.OnAttackHit += Action_OnAttackHit;
        }

        protected virtual void Action_OnAttackHit(object sender, AttackEventArgs args)
        {
            if (Equipment.WeaponSlot.CurrentWeapon != null) return;
            _damagedObject.Clear();

            Collider2D[] colliders = _damageHitBox.Hit(transform.position, Status.FacingDirectionValue);
            foreach (Collider2D collider in colliders)
            {
                if (!collider.enabled) continue;
                if (collider.transform == transform) continue; // Ignore self
                if (_damagedObject.Contains(collider.gameObject)) continue; // Avoid dealing damage to the same creature multiple times
                if (!collider.TryGetComponent(out ICreature creature)) continue;

                CombatSystem.DamageDealing(new DamageContainer(this, creature)
                {
                    Damage = Stats.Strength.Value * 0.4f,
                    DamageType = DamageType.Physical,
                });

                _damagedObject.Add(collider.gameObject);
            }
        }
    }
}