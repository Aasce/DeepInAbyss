using Asce.Managers.Utils;
using System.Collections.Generic;
using UnityEngine;


namespace Asce.Game.Enviroments
{
    public class PlatformPlat : Platform
    {
        [SerializeField] private BoxCollider2D _collider;

        [SerializeField] private Transform _leftEnd;
        [SerializeField] private Transform _rightEnd;

        [SerializeField] private Transform _middlePrefab;
        [SerializeField] private float _partSize = 1f;


        public virtual BoxCollider2D Collider
        {
            get => _collider;
            protected set => _collider = value;
        }

        public Transform LeftEnd => _leftEnd;
        public Transform RightEnd => _rightEnd;

        public Transform MiddlePrefab => _middlePrefab;
        public float PartSize => _partSize;


    }
}