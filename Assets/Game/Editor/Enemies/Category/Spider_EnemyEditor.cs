using Asce.Game.Entities.Enemies.Category;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{
    [CustomEditor(typeof(Spider_Enemy))]
    public class Spider_EnemyEditor : Editor
    {
        protected Spider_Enemy _enemy;

        protected virtual void OnEnable()
        {
            _enemy = (Spider_Enemy)target;
        }

        protected virtual void OnSceneGUI()
        {
            this.DrawHitBox();
        }


        protected virtual void DrawHitBox()
        {
            // Get the hitbox world position, size, and rotation angle
            Vector2 position = _enemy.DamageHitBox.GetPosition(_enemy.gameObject.transform.position, _enemy.Status.FacingDirectionValue == 0 ? 1 : _enemy.Status.FacingDirectionValue);
            Vector2 size = _enemy.DamageHitBox.GetSize();
            float angle = _enemy.DamageHitBox.GetAngle();
            SceneEditorUtils.DrawBox(position, size, angle);
        }
    }
}