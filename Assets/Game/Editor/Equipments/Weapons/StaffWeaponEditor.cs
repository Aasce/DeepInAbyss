using Asce.Game.Equipments.Weapons;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{
    [CustomEditor(typeof(StaffWeapon))]
    public class StaffWeaponEditor : WeaponEditor
    {
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnSceneGUI()
        {
            base.OnSceneGUI();
        }
    }
}