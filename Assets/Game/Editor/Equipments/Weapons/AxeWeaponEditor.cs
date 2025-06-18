using Asce.Game.Equipments.Weapons;
using UnityEditor;

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
            _axe.BladeHitBox.DrawHitBox(_owner.gameObject.transform.position, _owner.Status.FacingDirectionValue);
        }
    }
}