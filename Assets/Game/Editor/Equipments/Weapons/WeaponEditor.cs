using Asce.Game.Entities;
using Asce.Game.Equipments.Weapons;
using Codice.CM.Client.Differences;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{
    [CustomEditor(typeof(WeaponObject))]
    public class WeaponEditor : Editor
    {
        protected WeaponObject _weapon;
        protected ICreature _owner;

        protected virtual void OnEnable()
        {
            _weapon = (WeaponObject)target;
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

            // Get the hitbox world position, size, and rotation angle
            Vector2 position = _weapon.HitBox.GetPosition(_owner.gameObject.transform.position, _owner.Status.FacingDirectionValue == 0 ? 1 : _owner.Status.FacingDirectionValue);
            Vector2 size = _weapon.HitBox.GetSize();
            float angle = _weapon.HitBox.GetAngle();
            SceneEditorUtils.DrawBox(position, size, angle);
        }
    }
}