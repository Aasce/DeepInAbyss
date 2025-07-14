using Asce.Game.Enviroments;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{
    [CustomEditor(typeof(InteractiveObject), editorForChildClasses: true)]
    [CanEditMultipleObjects]
    public class InteractiveObjectEditor : Editor
    {
        private InteractiveObject _interactiveObject;

        private void OnEnable()
        {
            _interactiveObject = (InteractiveObject)target;
        }

        private void OnSceneGUI()
        {
            if (_interactiveObject == null) return;

            Vector2 position = (Vector2)_interactiveObject.transform.position + _interactiveObject.Offset;

            Handles.color = new Color(1f, 0.5f, 0f, 0.1f); // Orange transparent
            Handles.DrawSolidDisc(position, Vector3.forward, _interactiveObject.InteractionRange);

            Handles.color = Color.yellow;
            Handles.DrawWireDisc(position, Vector3.forward, _interactiveObject.InteractionRange);
        }
    }
}