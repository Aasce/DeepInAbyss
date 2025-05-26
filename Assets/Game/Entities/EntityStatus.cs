using System;
using UnityEngine;

namespace Asce.Game.Entities
{
    /// <summary>
    ///     Represents the status of an entity.
    /// </summary>
    [Serializable]
    public class EntityStatus
    {
        [SerializeField] private EntityStatusType _currentStatus = EntityStatusType.Alive;
        [SerializeField] protected FacingType _facingDirection = FacingType.None;

        public event Action<object> OnDeath;
        public event Action<object> OnRevive;

        public event Action<object, FacingType> OnFacingChanged;

        public EntityStatusType CurrentStatus => _currentStatus;
        public bool IsAlive => _currentStatus == EntityStatusType.Alive;
        public bool IsDead => _currentStatus == EntityStatusType.Dead;

        public FacingType FacingDirection
        {
            get => _facingDirection;
            set
            {
                if (value == FacingType.None) return;
                if (_facingDirection == value) return;

                _facingDirection = value;
                OnFacingChanged?.Invoke(this, _facingDirection);
            }
        }
        public int FacingDirectionValue
        {
            get => (int)FacingDirection;
            set => FacingDirection = (FacingType)value;
        }

        public void SetStatus(EntityStatusType state)
        {
            if (_currentStatus == state) return;
            _currentStatus = state;

            switch (_currentStatus)
            {
                case EntityStatusType.Dead:
                    OnDeath?.Invoke(this);
                    break;
                case EntityStatusType.Alive:
                    OnRevive?.Invoke(this);
                    break;
                default:
                    break;
            }
        }   
    }
}