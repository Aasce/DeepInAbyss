using UnityEngine;

namespace Asce.Game.Enviroments
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Ladder : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D _boxCollider2D;
        [SerializeField] private Transform _climbPosition;
        [SerializeField] private FacingType _direction = FacingType.Right;

        public FacingType Direction
        {
            get => _direction;
            set => _direction = value;
        }

        public Vector2 TopPosition => (Vector2)_boxCollider2D.bounds.center + new Vector2(0.0f, _boxCollider2D.size.y * 0.5f);
        public Vector2 BottomPosition => (Vector2)_boxCollider2D.bounds.center - new Vector2(0.0f, _boxCollider2D.size.y * 0.5f);
        public Vector3 ClimbPosition => _climbPosition.transform.position;


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
