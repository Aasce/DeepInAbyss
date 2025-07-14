using Asce.Game.Entities.Characters;
using Asce.Game.Enviroments;
using Asce.Managers.Utils;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{
    [CustomEditor(typeof(CharacterInteraction), true)]
    public class CharacterInteractionEditor : Editor
    {
        private CharacterInteraction _interaction;

        private void OnEnable()
        {
            _interaction = (CharacterInteraction)target;
        }

        private void OnSceneGUI()
        {
            if (_interaction == null) return;
            if (!Application.isPlaying) return;
            if (!_interaction.Owner.IsControled) return;

            foreach (IInteractableObject interactiveObject in _interaction.InteractableObjects)
            {
                if (interactiveObject == null) continue;

                Vector2 position = (Vector2)interactiveObject.gameObject.transform.position + interactiveObject.Offset;
                float range = interactiveObject.InteractionRange;

                if (interactiveObject  == _interaction.FocusObject)
                {
                    Handles.color = Color.green.WithAlpha(0.02f);
                    Handles.DrawSolidDisc(position, Vector3.forward, range);

                    Handles.color = Color.green;
                    Handles.DrawWireDisc(position, Vector3.forward, range);
                }
                else
                {
                    Handles.color = Color.blue.WithAlpha(0.02f);
                    Handles.DrawSolidDisc(position, Vector3.forward, range);

                    Handles.color = Color.blue;
                    Handles.DrawWireDisc(position, Vector3.forward, range);
                }
            }
        }
    }
}