using Asce.Game.Equipments.Weapons;
using Asce.Managers;
using System;
using UnityEngine;

namespace Asce.Game.Equipments
{
    public class WeaponSlot : MonoBehaviour, IEquipmentSlot
    {
        // Ref
        [SerializeField] protected IEquipmentController _equipmentOwner;

        [SerializeField] protected Weapon _currentWeapon;

        public event Action<object, ValueChangedEventArgs<Weapon>> OnWeaponChanged;

        public IEquipmentController EquipmentOwner
        {
            get => _equipmentOwner;
            set => _equipmentOwner = value;
        }

        public Weapon CurrentWeapon
        {
            get => _currentWeapon;
            protected set
            {
                if (_currentWeapon == value) return;
                Weapon oldWeapon = _currentWeapon;
                _currentWeapon = value;

                OnWeaponChanged?.Invoke(this, new ValueChangedEventArgs<Weapon>(oldWeapon, _currentWeapon));
            }
        }

        protected virtual void Start()
        {
            if (transform.childCount > 0)
            {
                // If there is a weapon already attached, set it as the current weapon
                if (transform.GetChild(0).TryGetComponent(out Weapon weapon))
                {
                    this.AddWeapon(weapon);
                }
            }
        }

        public void AddWeapon(Weapon weapon)
        {
            if (weapon == null) return;
            this.DetachWeapon(); // Detach any existing weapon before attaching a new one

            CurrentWeapon = weapon;
            CurrentWeapon.transform.SetParent(transform);
            CurrentWeapon.OnAttach(EquipmentOwner.Owner);
        }

        public void DetachWeapon()
        {
            if (CurrentWeapon == null) return;

            CurrentWeapon.OnDetach();

            CurrentWeapon.transform.SetParent(null);
            CurrentWeapon = null;
        }
    }
}
