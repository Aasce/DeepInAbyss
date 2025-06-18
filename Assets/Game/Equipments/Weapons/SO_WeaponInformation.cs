using Asce.Game.Combats;
using UnityEngine;

namespace Asce.Game.Equipments
{
    [CreateAssetMenu(menuName = "Asce/Items/Weapon Information", fileName = "Weapon Information")]
    public class SO_WeaponInformation : SO_EquipmentInformation
    {
        [Space]
        [SerializeField] private WeaponType _weaponType = WeaponType.None;
        [SerializeField] private AttackType _attackType = AttackType.None;
        [SerializeField] private AttackType _meleeAttackType = AttackType.Swipe;

        [Space]
        [SerializeField] private float _meleeDamageScale = 1f;


        public WeaponType WeaponType => _weaponType;
        public AttackType AttackType => _attackType;
        public AttackType MeleeAttackType => _meleeAttackType;

        public float MeleeDamageScale => _meleeDamageScale;
    }
}