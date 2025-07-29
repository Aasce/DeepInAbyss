using System;
using UnityEngine;

namespace Asce.Game.Enviroments
{
    public interface IInteractableObject : IGameObject
    {
        public event Action<object> OnFocus;
        public event Action<object> OnUnfocus;
        public event Action<object, GameObject> OnInteract;

        /// <summary> The maximum interaction distance from the player. </summary>
        public float InteractionRange { get; }
        public Vector2 Offset { get; }

        /// <summary> Called when the player interacts. </summary>
        public void Interact(GameObject interactor);


        public void Focus();
        public void Unfocus();
    }
}