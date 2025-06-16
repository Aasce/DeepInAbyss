using Asce.Game.Combats;
using Asce.Game.Equipments;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities
{
    public class CharacterEquipment : CreatureEquipment, IHasOwner<Character>, IHasWeaponSlot
    {
        [SerializeField] protected WeaponSlot _weaponSlot;

        public new Character Owner
        {
            get => base.Owner as Character;
            set => base.Owner = value;
        }

        public WeaponSlot WeaponSlot => _weaponSlot;

        protected override void Reset()
        {
            base.Reset();
            this.LoadComponent(out _weaponSlot);
        }

        protected override void Start()
        {
            base.Start();
            this.SetAttackType();
            WeaponSlot.OnWeaponChanged += WeaponSlot_OnWeaponChanged;
        }

        protected override void Status_OnDeath(object sender)
        {
            base.Status_OnDeath(sender);
            WeaponSlot.DetachWeapon();
        }

        private void WeaponSlot_OnWeaponChanged(object sender, Managers.ValueChangedEventArgs<Weapon> args)
        {
            if (args.OldValue != null) args.OldValue.Owner = null; // Clear the owner reference of the old weapon
            if (args.NewValue != null) args.NewValue.Owner = Owner; // Set the owner reference of the new weapon
            this.SetAttackType();
        }

        protected virtual void SetAttackType() => Owner.Action.AttackType = WeaponSlot.CurrentWeapon == null ? AttackType.Swipe : WeaponSlot.CurrentWeapon.AttackType;
    }
}