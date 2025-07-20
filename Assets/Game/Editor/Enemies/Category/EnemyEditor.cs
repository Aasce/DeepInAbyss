using Asce.Game.Entities.Enemies;
using Asce.Game.Stats;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{
    [CustomEditor(typeof(Enemy), editorForChildClasses: true)]
    public class EnemyEditor : EntityEditor
    {
        protected Enemy _enemy;

        protected override void OnEnable()
        {
            base.OnEnable();
            _enemy = (Enemy)target;
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
            Vector2 position = _enemy.DamageHitBox.GetPosition(_enemy.gameObject.transform.position, _enemy.Status.FacingDirectionValue == 0 ? 1 : _enemy.Status.FacingDirectionValue);
            Vector2 size = _enemy.DamageHitBox.GetSize();
            float angle = _enemy.DamageHitBox.GetAngle();
            SceneEditorUtils.DrawBox(position, size, angle);
        }

        protected virtual void DrawViewRadius()
        {
            if (_enemy == null || _enemy.Stats == null) return;
            if (_enemy.Stats is not IHasViewRadius hasViewRadius) return;

            Handles.color = Color.yellow;
            Handles.DrawWireDisc(_enemy.transform.position, Vector3.forward, hasViewRadius.ViewRadius.Value);
        }

    }
}