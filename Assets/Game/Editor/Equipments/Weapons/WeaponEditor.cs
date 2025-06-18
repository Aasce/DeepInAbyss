using Asce.Game.Entities;
using Asce.Game.Equipments.Weapons;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{
    [CustomEditor(typeof(Weapon))]
    public class WeaponEditor : Editor
    {
        protected Weapon _weapon;
        protected ICreature _owner;

        protected virtual void OnEnable()
        {
            _weapon = (Weapon)target;
            if (_weapon.Owner != null) _owner = _weapon.Owner;
            else _owner = _weapon.gameObject.GetComponentInParent<ICreature>();
        }

        protected virtual void OnSceneGUI()
        {
            this.DrawHitBox();
        }

        protected virtual void DrawHitBox()
        {
            if (_owner == null) return;
            _weapon.HitBox.DrawHitBox(_owner.gameObject.transform.position, _owner.Status.FacingDirectionValue);
        }
    }
}