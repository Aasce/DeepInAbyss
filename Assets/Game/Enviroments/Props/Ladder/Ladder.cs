using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Enviroments
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Ladder : MonoBehaviour, IEnviromentComponent
    {
        [SerializeField] private BoxCollider2D _boxCollider2D;
        [SerializeField] private Vector3 _climbOffset;

        [Header("Ladder Parts")]
        [SerializeField] private Transform _topPart;
        [SerializeField] private Transform _bottomPart;
        [SerializeField] private List<Transform> _middleParts;

        [SerializeField] private float _partSize = 1f;

        [Space]
        [SerializeField] private FacingType _direction = FacingType.Right;


        public BoxCollider2D Collider => _boxCollider2D;

        public Transform TopPart => _topPart;
        public Transform BottomPart => _bottomPart;
        public List<Transform> MiddleParts => _middleParts;
        public float PartSize => _partSize;

        public FacingType Direction
        {
            get => _direction;
            set => _direction = value;
        }

        public Vector2 TopPosition => (Vector2)_boxCollider2D.bounds.center + new Vector2(0.0f, _boxCollider2D.size.y * 0.5f);
        public Vector2 BottomPosition => (Vector2)_boxCollider2D.bounds.center - new Vector2(0.0f, _boxCollider2D.size.y * 0.5f);
        public Vector3 ClimbPosition => transform.position + _climbOffset;


        protected virtual void Awake()
        {
            if (_boxCollider2D == null) _boxCollider2D = GetComponent<BoxCollider2D>();
        }

        protected virtual void Start()
        {
            if (Direction == FacingType.None) Direction = FacingType.Right;
        }
    }
}
