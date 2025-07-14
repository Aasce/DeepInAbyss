using UnityEngine;

namespace Asce.Game.Enviroments
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Sign : InteractiveObject, IEnviromentComponent, IInteractableObject
    {

        public override void Interact(GameObject interactor)
        {

        }

        public override void OnFocusEnter()
        {
            base.OnFocusEnter();

        }

        public override void OnFocusExit()
        {
            base.OnFocusExit();

        }

    }
}
