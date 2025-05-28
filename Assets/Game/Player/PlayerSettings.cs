using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Players
{
    public class PlayerSettings : MonoBehaviour
    {
        [SerializeField] private LayerMask _mouseLayerMask = default;

        [Header("Control Character")]
        [SerializeField] private KeyCode _runKey = KeyCode.LeftShift;
        [SerializeField] private KeyCode _dashKey = KeyCode.LeftControl;

        [SerializeField] private KeyCode _dodgeKey = KeyCode.C;
        [SerializeField] private KeyCode _crounchKey = KeyCode.X;
        [SerializeField] private KeyCode _crawlKey = KeyCode.Z;

        [Space]
        [SerializeField] private KeyCode _attackKey = KeyCode.Mouse0;

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

        public LayerMask MouseLayerMask
        {
            get => _mouseLayerMask;
            set => _mouseLayerMask = value;
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

        public KeyCode LookKey
        {
            get => _lookKey;
            set => _lookKey = value;
        }

        public List<KeyCode> UseToolKeys => _useToolKeys;

        public List<KeyCode> UseItemKeys => _useItemKeys;
    }
}
