using Asce.Game.Equipments.Weapons;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{
    [CustomEditor(typeof(BowWeapon))]
    public class BowWeaponEditor : WeaponEditor
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