using Asce.Managers;
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
        protected IEntity _entity;

        [SerializeField] protected EntityStatusType _currentStatus = EntityStatusType.Alive;
        [SerializeField] protected FacingType _facingDirection = FacingType.None;
        [SerializeField] protected float _height = 2f;
        [SerializeField] protected Vector2 _spawnPosition = Vector2.zero;

        public event Action<object> OnDeath;
        public event Action<object> OnRevive;

        public event Action<object, FacingType> OnFacingChanged;
        public event Action<object, ValueChangedEventArgs> OnHeightChanged;

        public IEntity Entity
        {
            get => _entity;
            set => _entity = value;
        }
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
        public float Height
        {
            get => _height;
            set
            {
                if (value == _height) return;
                float oldHeight = _height;
                _height = value;
                OnHeightChanged?.Invoke(this, new ValueChangedEventArgs(oldHeight, _height));
            }
        }

        public Vector2 SpawnPosition
        {
            get => _spawnPosition;
            set => _spawnPosition = value;
        }


        public void SetStatus(EntityStatusType state)
        {
            if (_currentStatus == state) return;
            _currentStatus = state;

            switch (_currentStatus)
            {
                case EntityStatusType.Dead:
                    OnDeath?.Invoke(Entity);
                    break;
                case EntityStatusType.Alive:
                    OnRevive?.Invoke(Entity);
                    break;
                default:
                    break;
            }
        }   
    }
}