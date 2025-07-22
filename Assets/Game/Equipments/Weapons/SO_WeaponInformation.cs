using Asce.Game.Combats;
using Asce.Game.Equipments.Weapons;
using UnityEngine;

namespace Asce.Game.Equipments
{
    [CreateAssetMenu(menuName = "Asce/Items/Sender Information", fileName = "Sender Information")]
    public class SO_WeaponInformation : SO_EquipmentInformation
    {
        [Space]
        [SerializeField] protected WeaponType _weaponType = WeaponType.None;
        [SerializeField] protected AttackType _attackType = AttackType.None;
        [SerializeField] protected AttackType _meleeAttackType = AttackType.Swipe;

        [Space]
        [SerializeField] protected float _meleeDamageScale = 1f;

        [Space]
        [SerializeField] protected WeaponObject _weaponObject;

        public WeaponType WeaponType => _weaponType;
        public AttackType AttackType => _attackType;
        public AttackType MeleeAttackType => _meleeAttackType;

        public float MeleeDamageScale => _meleeDamageScale;

        public WeaponObject WeaponObject => _weaponObject;
    }
}