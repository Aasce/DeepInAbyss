using Asce.Game.Entities.Characters;
using Asce.Game.Stats;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{
    [CustomEditor(typeof(Character), editorForChildClasses: true)]
    public class CharacterEditor : EntityEditor
    {
        protected Character _character;

        protected override void OnEnable()
        {
            base.OnEnable();
            _character = (Character)target;
        }

        protected override void OnSceneGUI()
        {
            base.OnSceneGUI();
            this.DrawHitBox();
            this.DrawViewRadius();
        }


        protected virtual void DrawHitBox()
        {
            // Get the hitbox world position, size, and rotation angle
            Vector2 position = _character.DamageHitBox.GetPosition(_character.gameObject.transform.position, _character.Status.FacingDirectionValue == 0 ? 1 : _character.Status.FacingDirectionValue);
            Vector2 size = _character.DamageHitBox.GetSize();
            float angle = _character.DamageHitBox.GetAngle();
            SceneEditorUtils.DrawBox(position, size, angle);
        }

        protected virtual void DrawViewRadius()
        {
            if (_character == null || _character.Stats == null) return;
            if (_character.Stats is not IHasViewRadius hasViewRadius) return;

            Handles.color = Color.yellow;
            Handles.DrawWireDisc(_character.transform.position, Vector3.forward, hasViewRadius.ViewRadius.Value);
        }

    }
}