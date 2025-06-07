using System;
using UnityEngine;

namespace Asce.Game.Entities.Enemies
{
    public class EnemyAnimationEventReceiver : MonoBehaviour, IHasOwner<Enemy>
    {
        [SerializeField] private Enemy _owner;

        public event Action<object, Enemy> OnFootstepEvent;
        public event Action<object, Enemy> OnAttackEvent;
        public event Action<object, Enemy> OnDieFxEvent;

        public Enemy Owner
        {
            get => _owner;
            set => _owner = value;
        }

        public void OnFootstep()
        {
            OnFootstepEvent?.Invoke(this, Owner);
        }

        public void OnAttack()
        {
            OnAttackEvent?.Invoke(this, Owner);
        }

        public void OnDieFx()
        {
            OnDieFxEvent?.Invoke(this, Owner);
        }
    }
}
