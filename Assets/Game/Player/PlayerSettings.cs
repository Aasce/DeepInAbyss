using Asce.Managers;
using Asce.Managers.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Players
{
    public class PlayerSettings : GameComponent, IPlayerComponent
    {
        // Ref
        [SerializeField, Readonly] private Player _player;

        [SerializeField] private LayerMask _mouseLayerMask = default;
        [SerializeField] private LayerMask _interactiveLayerMask;
        [SerializeField] private LayerMask _renderCreatureLayerMask;

        [Header("Control Creature")]
        [SerializeField] private KeyCode _runKey = KeyCode.LeftShift;
        [SerializeField] private KeyCode _dashKey = KeyCode.LeftControl;

        [SerializeField] private KeyCode _dodgeKey = KeyCode.C;
        [SerializeField] private KeyCode _crounchKey = KeyCode.X;
        [SerializeField] private KeyCode _crawlKey = KeyCode.Z;

        [Space]
        [SerializeField] private KeyCode _attackKey = KeyCode.Mouse0;
        [SerializeField] private KeyCode _meleeAttackKey = KeyCode.F;
        [SerializeField] private KeyCode _detachWeaponKey = KeyCode.G;

        [Space]
        [SerializeField] private KeyCode _lookKey = KeyCode.Mouse1;

        [Space]
        [SerializeField] private List<KeyCode> _useToolKeys = new() 
        {
            KeyCode.E,
            KeyCode.Q
        };

        [SerializeField] private List<KeyCode> _useItemKeys = new()
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2, 
            KeyCode.Alpha3,
            KeyCode.Alpha4,
        };

        [Header("Control UI")]
        [SerializeField] private KeyCode _backUIKey = KeyCode.Escape;
        [SerializeField] private KeyCode _inventoryWindowKey = KeyCode.B;

        [Header("Interactions")]
        [SerializeField] private KeyCode _interactionKey = KeyCode.F;

        public Player Player
        {
            get => _player;
            set => _player = value;
        }


        public LayerMask MouseLayerMask
        {
            get => _mouseLayerMask;
            set => _mouseLayerMask = value;
        }

        public LayerMask InteractiveLayerMask
        {
            get => _interactiveLayerMask;
            set => _interactiveLayerMask = value;
        }

        public LayerMask RenderCreatureLayer
        {
            get => _renderCreatureLayerMask;
            set => _renderCreatureLayerMask = value;
        }

        public KeyCode RunKey
        {
            get => _runKey;
            set => _runKey = value;
        }

        public KeyCode DashKey
        {
            get => _dashKey;
            set => _dashKey = value;
        }

        public KeyCode DodgeKey
        {
            get => _dodgeKey;
            set => _dodgeKey = value;
        }

        public KeyCode CrounchKey
        {
            get => _crounchKey;
            set => _crounchKey = value;
        }

        public KeyCode CrawlKey
        {
            get => _crawlKey;
            set => _crawlKey = value;
        }

        public KeyCode AttackKey
        {
            get => _attackKey; 
            set => _attackKey = value;
        }

        public KeyCode MeleeAttackKey
        {
            get => _meleeAttackKey; 
            set => _meleeAttackKey = value;
        }

        public KeyCode DetachWeaponKey
        {
            get => _detachWeaponKey; 
            set => _detachWeaponKey = value;
        }

        public KeyCode LookKey
        {
            get => _lookKey;
            set => _lookKey = value;
        }

        public List<KeyCode> UseToolKeys => _useToolKeys;
        public List<KeyCode> UseItemKeys => _useItemKeys;

        public KeyCode BackUIKey
        {
            get => _backUIKey;
            set => _backUIKey = value;
        }
        public KeyCode InventoryWindowKey
        {
            get => _inventoryWindowKey;
            set => _inventoryWindowKey = value;
        }

        public KeyCode InteractionKey
        {
            get => _interactionKey;
            set => _interactionKey = value;
        }
    }
}
