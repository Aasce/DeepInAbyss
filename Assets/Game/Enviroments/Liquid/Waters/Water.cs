using Asce.Game.Entities;
using Asce.Game.Stats;
using Asce.Game.StatusEffects;
using UnityEngine;

namespace Asce.Game.Enviroments
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(BoxCollider2D))]
    public class Water : Liquid
    {
        protected override void Start()
        {
            base.Start();
            if (_triggerHandler != null)
            {
                _triggerHandler.OnObjectEnter += TriggerHandler_OnObjectEnter;
                _triggerHandler.OnObjectExit += TriggerHandler_OnObjectExit;
            }
        }


        protected virtual void TriggerHandler_OnObjectEnter(object sender, Collider2D collider)
        {
            if (collider == null) return;
            if (!collider.TryGetComponent(out ICreature creature)) return;
            if (creature.Stats is not IHasSustenance hasSuustenance) return;
            StatusEffectsManager.Instance.SendEffect<Underwater_StatusEffect>(null, creature, new EffectDataContainer()
            {
                Strength = 3f,
                Duration = float.PositiveInfinity,
            });
        }

        protected virtual void TriggerHandler_OnObjectExit(object sender, Collider2D collider)
        {
            if (collider == null) return;
            if (!collider.TryGetComponent(out ICreature creature)) return;
            if (creature.Stats is not IHasSustenance hasSuustenance) return;
            StatusEffectsManager.Instance.RemoveEffect<Underwater_StatusEffect>(creature);
        }
    }
}