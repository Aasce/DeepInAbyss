using Asce.Game.Equipments.Weapons;
using Asce.Game.Items;
using Asce.Managers;
using System;
using UnityEngine;

namespace Asce.Game.Equipments
{
    public class WeaponSlot : EquipmentSlot
    {
        [SerializeField] protected WeaponObject _currentWeapon;

        public event Action<object, ValueChangedEventArgs<WeaponObject>> OnWeaponChanged;

        public WeaponObject CurrentWeapon
        {
            get => _currentWeapon;
            protected set
            {
                if (_currentWeapon == value) return;
                WeaponObject oldWeapon = _currentWeapon;
                _currentWeapon = value;

                OnWeaponChanged?.Invoke(this, new ValueChangedEventArgs<WeaponObject>(oldWeapon, _currentWeapon));
            }
        }

        protected override void Register()
        {
            base.Register();
            if (_equipmentItem.IsNull()) return;
            if (_equipmentItem.Information.Type != ItemType.Weapon) return;
            if (_equipmentItem.Information is not SO_WeaponInformation weaponInformation) return;

            if (weaponInformation.WeaponObject == null) return;
            this.SetWeapon(weaponInformation.WeaponObject);
        }

        protected override void Unregister()
        {
            base.Unregister();
            this.DetachWeapon(); // Detach the weapon when unregistering
        }

        protected void SetWeapon(WeaponObject weapon)
        {
            if (weapon == null) return;
            this.DetachWeapon(); // Detach any existing weapon before attaching a new one

            CurrentWeapon = Instantiate(weapon);
            CurrentWeapon.transform.SetParent(transform);
            CurrentWeapon.OnAttach(EquipmentOwner.Owner);
        }

        protected void DetachWeapon()
        {
            if (CurrentWeapon == null) return;

            CurrentWeapon.OnDetach();
            WeaponObject weapon = CurrentWeapon;
            Destroy(weapon.gameObject); // Destroy the weapon instance after detaching it
            CurrentWeapon = null;
        }
    }
}
