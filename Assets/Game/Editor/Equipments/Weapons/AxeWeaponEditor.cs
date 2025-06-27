using Asce.Game.Equipments.Weapons;
using Codice.CM.Client.Differences;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{
    [CustomEditor(typeof(AxeWeapon))]
    public class AxeWeaponEditor : WeaponEditor
    {
        protected AxeWeapon _axe;

        protected override void OnEnable()
        {
            base.OnEnable();
            _axe = (AxeWeapon)target;
        }

        protected override void OnSceneGUI()
        {
            if (_owner == null) return;

            base.OnSceneGUI();

            // Get the hitbox world position, size, and rotation angle
            Vector2 position = _axe.BladeHitBox.GetPosition(_owner.gameObject.transform.position, _owner.Status.FacingDirectionValue == 0 ? 1 : _owner.Status.FacingDirectionValue);
            Vector2 size = _axe.BladeHitBox.GetSize();
            float angle = _axe.BladeHitBox.GetAngle();
            SceneEditorUtils.DrawBox(position, size, angle);
        }
    }
}