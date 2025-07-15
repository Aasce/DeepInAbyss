using Asce.Game.Entities;
using Asce.Game.Entities.Characters;
using Asce.Game.Enviroments;
using Asce.Managers.Pools;
using Asce.Managers.UIs;
using System;
using UnityEngine;

namespace Asce.Game.UIs
{
    public class UIInteractableObjectController : UIObject
    {
        [SerializeField] protected Pool<UIInteractableObjectInformation> _pool = new();
        protected ICreature _creature;

        protected override void RefReset()
        {
            base.RefReset();
        }

        public void SetCreature(ICreature creature)
        {
            if (_creature == creature) return;

            this.Unregister();
            _creature = creature;
            this.Register();
        }

        protected virtual void Register()
        {
            if (_creature == null) return;
            if (_creature is not Character character) return;
            if (character.Interaction == null) return;

            character.Interaction.OnInteractableObjectAdded += Interaction_OnInteractableObjectAdded;
            character.Interaction.OnInteractableObjectRemoved += Interaction_OnInteractableObjectRemoved;
        }

        protected virtual void Unregister()
        {
            if (_creature == null) return;
            if (_creature is not Character character) return;
            if (character.Interaction == null) return;

            character.Interaction.OnInteractableObjectAdded -= Interaction_OnInteractableObjectAdded;
            character.Interaction.OnInteractableObjectRemoved -= Interaction_OnInteractableObjectRemoved;
        }

        protected virtual void Interaction_OnInteractableObjectAdded(object sender, IInteractableObject interactableObject)
        {
            if (interactableObject == null) return;
            UIInteractableObjectInformation uiInteractableObject = _pool.Activate();
            if (uiInteractableObject == null) return;

            uiInteractableObject.Set(interactableObject);
            uiInteractableObject.Show();
        }

        protected virtual void Interaction_OnInteractableObjectRemoved(object sender, IInteractableObject interactableObject)
        {
            if (interactableObject == null) return;

            UIInteractableObjectInformation uiInteractableObject = _pool.Activities.Find(ui => ui != null && ui.InteractableObject == interactableObject);
            if (uiInteractableObject == null) return;

            uiInteractableObject.Hide();
            _pool.Deactivate(uiInteractableObject);
        }
    }
}
