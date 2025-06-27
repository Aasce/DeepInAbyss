using Asce.Game.Combats;
using Asce.Game.Equipments.Weapons;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{
    [CustomEditor(typeof(SwordWeapon))]
    public class SwordWeaponEditor : WeaponEditor
    {
        protected SwordWeapon _sword;

        protected override void OnEnable()
        {
            base.OnEnable();
            _sword = (SwordWeapon)target;
        }

        protected override void OnSceneGUI()
        {
            if (_owner == null) return;

            base.OnSceneGUI();

            // Get the hitbox world position, size, and rotation angle
            Vector2 position = _sword.StabHitBox.GetPosition(_owner.gameObject.transform.position, _owner.Status.FacingDirectionValue == 0 ? 1 : _owner.Status.FacingDirectionValue);
            Vector2 size = _sword.StabHitBox.GetSize();
            float angle = _sword.StabHitBox.GetAngle();
            SceneEditorUtils.DrawBox(position, size, angle);
        }
    }
}