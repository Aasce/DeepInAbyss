using Asce.BehaviourTrees;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities.AIs
{
    public class CreatureAI : MonoBehaviour
    {
        [SerializeField] protected Creature _creature;
        protected BehaviourTree _behaviour = new();

        public Creature Creature
        {
            get => _creature;
            set
            {
                if (_creature == value) return;

                if (_creature != null) this.Unregister();
                _creature = value;
                if (_creature != null) this.Register();
            }
        }

        public BehaviourTree Behaviour => _behaviour;


        protected virtual void Awake()
        {
            if (Creature == null) this.LoadComponent(out _creature);
        }

        protected virtual void Start()
        {
            this.CreateBehaviour();
            this.Register();
        }

        protected virtual void Update()
        {
            Behaviour.Tick();
        }

        protected virtual void CreateBehaviour()
        {
            
        }

        protected virtual void Register()
        {
            if (Creature == null) return;

            _behaviour.Blackboard.Set<Creature>("Self", Creature);
            Creature.Status.OnDeath += CreatureStatus_OnDeath;
            Creature.Status.OnRevive += CreatureStatus_OnRevive;
        }

        protected virtual void Unregister()
        {
            if (Creature == null) return;
            
            Creature.Status.OnDeath -= CreatureStatus_OnDeath;
            Creature.Status.OnRevive -= CreatureStatus_OnRevive;
            _behaviour.Blackboard.Set<Creature>("Self", null);

            Creature = null;
        }

        protected virtual void CreatureStatus_OnDeath(object sender)
        {
            _behaviour.IsTickEnabled = false;
        }

        protected virtual void CreatureStatus_OnRevive(object sender)
        {
            _behaviour.Reset();
            _behaviour.IsTickEnabled = true;
        }
    }
}