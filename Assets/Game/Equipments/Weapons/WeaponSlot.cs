using Asce.Managers;
using System;
using UnityEngine;

namespace Asce.Game.Equipments
{
    public class WeaponSlot : MonoBehaviour
    {
        [SerializeField] protected Weapon _currentWeapon;

        public event Action<object, ValueChangedEventArgs<Weapon>> OnWeaponChanged;


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

            CurrentWeapon.Collider.isTrigger = true;
            CurrentWeapon.Rigidbody.bodyType = RigidbodyType2D.Kinematic;

            CurrentWeapon.transform.SetParent(transform);
            CurrentWeapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            CurrentWeapon.transform.localScale = Vector3.one;
        }

        public void DetachWeapon()
        {
            if (CurrentWeapon == null) return;

            CurrentWeapon.Collider.isTrigger = false;
            CurrentWeapon.Rigidbody.bodyType = RigidbodyType2D.Dynamic;

            CurrentWeapon.transform.SetParent(null);
            CurrentWeapon = null;
        }
    }
}
