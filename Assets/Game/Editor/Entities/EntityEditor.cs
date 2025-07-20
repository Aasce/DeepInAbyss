using Asce.Game;
using Asce.Game.Entities;
using Asce.Managers.Utils;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{
    [CustomEditor(typeof(Entity), editorForChildClasses: true)]
    public class EntityEditor : Editor
    {
        protected Entity _entity;

        protected virtual void OnEnable()
        {
            _entity = (Entity)target;
        }

        protected virtual void OnSceneGUI()
        {
            this.DrawBounds();
        }

        private void DrawBounds()
        {
            if (_entity == null) return;
            SceneEditorUtils.DrawBounds((_entity as IOptimizedComponent).Bounds, Color.green, Color.green.WithAlpha(0.01f));
        }
    }
}