using Asce.Game.UIs;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities
{
    public class CreatureUI : MonoBehaviour, IHasOwner<Creature>, IWorldUI
    {
        [SerializeField, HideInInspector] private Creature _owner; 
        [SerializeField, HideInInspector] protected Canvas _canvas;
        [SerializeField] protected bool _isHideOnDead = true;

        public Creature Owner 
        { 
            get => _owner;
            set => _owner = value;
        }

        public Canvas Canvas => _canvas;

        public virtual bool IsHideOnDead
        {
            get => _isHideOnDead;
            set => _isHideOnDead = value;
        }


        protected virtual void Reset()
        {
            this.LoadComponent(out _canvas); 
            if (this.LoadComponent(out _owner))
            {

            }
        }

        protected virtual void Awake() { }
        protected virtual void Start()
        {
            Owner.Status.OnDeath += Status_OnDeath;
            Owner.Status.OnRevive += Status_OnRevive;
        }

        protected virtual void Status_OnDeath(object sender)
        {
            if (IsHideOnDead) Canvas.gameObject.SetActive(false);
        }
        protected virtual void Status_OnRevive(object sender)
        {
            if (IsHideOnDead) Canvas.gameObject.SetActive(true);
        }

    }
}