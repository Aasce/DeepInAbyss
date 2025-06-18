using Asce.Game.Equipments.Weapons;
using UnityEditor;

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
            _sword.StabHitBox.DrawHitBox(_owner.gameObject.transform.position, _owner.Status.FacingDirectionValue);
        }
    }
}