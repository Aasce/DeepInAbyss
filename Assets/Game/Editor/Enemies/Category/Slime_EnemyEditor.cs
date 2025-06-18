using Asce.Game.Entities.Enemies.Category;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{
    [CustomEditor(typeof(Slime_Enemy))]
    public class Slime_EnemyEditor : Editor
    {
        protected Slime_Enemy _enemy;

        protected virtual void OnEnable()
        {
            _enemy = (Slime_Enemy)target;
        }

        protected virtual void OnSceneGUI()
        {
            this.DrawHitBox();
        }


        protected virtual void DrawHitBox()
        {
            _enemy.DamageHitBox.DrawHitBox(_enemy.gameObject.transform.position, _enemy.Status.FacingDirectionValue);
        }
    }
}