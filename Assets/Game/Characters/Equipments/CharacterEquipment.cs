using Asce.Game.Combats;
using Asce.Game.Equipments;
using Asce.Game.Equipments.Weapons;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities
{
    public class CharacterEquipment : CreatureEquipment, IHasOwner<Character>, IEquipmentController
    {
        [SerializeField] protected WeaponSlot _weaponSlot;
        [SerializeField] protected OtherSlot _leftHandSlot;

        public new Character Owner
        {
            get => base.Owner as Character;
            set => base.Owner = value;
        }

        public WeaponSlot WeaponSlot => _weaponSlot;
        public OtherSlot LeftHandSlot => _leftHandSlot;

        protected override void Reset()
        {
            base.Reset();
            this.LoadComponent(out _weaponSlot);
            this.LoadComponent(out _leftHandSlot);
        }
        protected override void Awake()
        {
            base.Awake();
            WeaponSlot.EquipmentOwner = this;
            LeftHandSlot.EquipmentOwner = this;
        }
        protected override void Start()
        {
            base.Start();
            this.SetAttackType();
            if (WeaponSlot.CurrentWeapon != null) WeaponSlot.CurrentWeapon.Owner = Owner; // Set the owner reference of the current weapon

            WeaponSlot.OnWeaponChanged += WeaponSlot_OnWeaponChanged;

            Owner.Action.OnAttackStart += Action_OnAttackStart;
            Owner.Action.OnAttackHit += Action_OnAttackHit;
            Owner.Action.OnAttackEnd += Action_OnAttackEnd;
            Owner.Action.OnAttackCast += Action_OnAttackCast; // For weapons like staff
        }


        protected virtual void Update()
        {
            if (WeaponSlot.CurrentWeapon is BowWeapon bow) bow.View.UpdateStringPullPosition(Owner.View.RigHandL.position);
        }

        protected virtual void FixedUpdate()
        {
            this.UpdateEquipmentSlot();
        }

        public virtual void CreateProjectile()
        {
            if (LeftHandSlot.Projectile != null) return; // Already has a projectile

            BowWeapon bow = (Owner.Equipment.WeaponSlot.CurrentWeapon as BowWeapon);
            if (bow == null) return;

            Projectile projectile = Instantiate(bow.ArrowPrefab);
            projectile.Owner = Owner;
            LeftHandSlot.HoldProjectile(projectile);
        }

        protected virtual void UpdateEquipmentSlot()
        {
            Quaternion quaternion = Quaternion.Euler(0.0f, 0.0f, 180.0f);
            WeaponSlot.transform.SetPositionAndRotation(Owner.View.RigWeapon.transform.position, Owner.View.RigWeapon.transform.rotation * quaternion);
            LeftHandSlot.transform.SetPositionAndRotation(Owner.View.RigHandL.transform.position, Owner.View.RigHandL.transform.rotation);
        }

        protected override void Status_OnDeath(object sender)
        {
            base.Status_OnDeath(sender);
            WeaponSlot.DetachWeapon();
        }

        protected virtual void WeaponSlot_OnWeaponChanged(object sender, Managers.ValueChangedEventArgs<Weapon> args)
        {
            if (args.OldValue != null) args.OldValue.Owner = null; // Clear the owner reference of the old weapon
            if (args.NewValue != null) args.NewValue.Owner = Owner; // Set the owner reference of the new weapon
            this.SetAttackType();
        }

        protected virtual void Action_OnAttackStart(object sender, AttackEventArgs args)
        {
            if (WeaponSlot.CurrentWeapon == null) return;
            WeaponSlot.CurrentWeapon.StartAttacking(args.AttackType);
        }
        protected virtual void Action_OnAttackHit(object sender, AttackEventArgs args)
        {
            if (WeaponSlot.CurrentWeapon == null) return;
            WeaponSlot.CurrentWeapon.Attacking();
        }
        protected virtual void Action_OnAttackEnd(object sender, AttackEventArgs args)
        {
            if (WeaponSlot.CurrentWeapon == null) return;
            WeaponSlot.CurrentWeapon.EndAttacking(args.AttackType);
        }

        protected virtual void Action_OnAttackCast(object sender, Vector2 direction)
        {
            StaffWeapon weapon = Owner.Equipment.WeaponSlot.CurrentWeapon as StaffWeapon;
            if (weapon == null) return;

            Vector2 position = Owner.Equipment.WeaponSlot.CurrentWeapon.TipPosition;
            weapon.Cast(position, direction);
        }


        protected virtual void SetAttackType()
        {
            if (WeaponSlot.CurrentWeapon == null)
            {
                Owner.Action.AttackType = AttackType.Swipe;
                Owner.Action.MeleeAttackType = AttackType.Swipe;
                return;
            }

            Owner.Action.AttackType = WeaponSlot.CurrentWeapon.AttackType;
            Owner.Action.MeleeAttackType = WeaponSlot.CurrentWeapon.MeleeAttackType;
        }
    }
}