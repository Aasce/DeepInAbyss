using UnityEngine;

namespace Asce.Game.Combats
{
    /// <summary>
    ///     Represents a 2D hitbox that can detect collisions using Physics2D.OverlapBoxAll.
    /// </summary>
    [System.Serializable]
    public class HitBox
    {
        [SerializeField] protected LayerMask _layer;
        [SerializeField] protected Vector2 _offset;
        [SerializeField] protected Vector2 _size;
        [SerializeField] protected float _angle;

        /// <summary>
        ///     Gets or sets the LayerMask used for collision detection.
        /// </summary>
        public LayerMask Layer
        {
            get => _layer;
            set => _layer = value;
        }

        /// <summary>
        ///     Gets or sets the local offset from the owner’s position.
        /// </summary>
        public Vector2 Offset
        {
            get => _offset;
            set => _offset = value;
        }

        /// <summary>
        ///     Gets or sets the size of the hitbox.
        /// </summary>
        public Vector2 Size
        {
            get => _size;
            set => _size = value;
        }

        /// <summary>
        ///     Gets or sets the rotation angle (in degrees) of the hitbox.
        /// </summary>
        public float Angle
        {
            get => _angle;
            set => _angle = value;
        }

        /// <summary>
        ///     Default constructor. Initializes a hitbox with default values (zero offset, size one, angle 0).
        /// </summary>
        public HitBox() : this(Vector2.zero, Vector2.one, default, 0f) { }

        /// <summary>
        ///     Initializes a hitbox with the given offset, size, layer, and angle.
        /// </summary>
        /// <param name="offset"> Local offset from the owner’s position. </param>
        /// <param name="size"> Size of the hitbox. </param>
        /// <param name="layer"> LayerMask for collision filtering. </param>
        /// <param name="angle"> Rotation angle in degrees. </param>
        public HitBox(Vector2 offset, Vector2 size, LayerMask layer, float angle)
        {
            _offset = offset;
            _size = size;
            _layer = layer;
            _angle = angle;
        }

        /// <summary>
        ///     Calculates the world position of the hitbox based on the owner's position and facing direction.
        /// </summary>
        /// <param name="ownerPosition"> The world position of the owner. </param>
        /// <param name="ownerFacing">
        ///     The facing direction of the owner: typically 1 (right) or -1 (left).
        ///     This flips the X offset when facing left.
        /// </param>
        /// <returns> The calculated world position of the hitbox. </returns>
        public Vector2 GetPosition(Vector2 ownerPosition, int ownerFacing = 1)
        {
            // Flip X offset based on facing direction (e.g. left or right)
            Vector2 offset = new Vector2(_offset.x * ownerFacing, _offset.y);
            return ownerPosition + offset;
        }

        /// <summary>
        ///     Returns the current size of the hitbox.
        /// </summary>
        public Vector2 GetSize() => _size;

        /// <summary>
        ///     Returns the current angle of the hitbox.
        /// </summary>
        public float GetAngle() => _angle;

        /// <summary>
        ///     Performs a hit detection using OverlapBoxAll at the computed position.
        /// </summary>
        /// <param name="ownerPosition"> The world position of the hitbox owner. </param>
        /// <param name="ownerFacing"> The facing direction of the owner (1 or -1). </param>
        /// <returns> An array of colliders detected within the hitbox area. </returns>
        public Collider2D[] Hit(Vector2 ownerPosition, int ownerFacing = 1)
        {
            Vector2 position = GetPosition(ownerPosition, ownerFacing);
            Vector2 size = GetSize();
            float angle = GetAngle();
            LayerMask layer = Layer;

            // Use Physics2D to detect all overlapping colliders within the box
            Collider2D[] colliders = Physics2D.OverlapBoxAll(position, size, angle, layer);
            return colliders;
        }
    }
}