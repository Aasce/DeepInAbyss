using UnityEngine;

namespace Asce.Game.Enviroments
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Sign : InteractiveObject, IEnviromentComponent, IInteractableObject
    {

        public override void Interact(GameObject interactor)
        {

        }

        public override void Focus()
        {
            base.Focus();

        }

        public override void Unfocus()
        {
            base.Unfocus();

        }

    }
}
