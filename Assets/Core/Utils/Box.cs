using UnityEngine;

namespace Asce.Managers.Utils
{
    /// <summary>
    /// Represents a 2D box with a position and size.
    /// </summary>
    [System.Serializable]
    public class Box
    {
        [SerializeField] private Vector2 _offset;
        [SerializeField] private Vector2 _size;

        public Vector2 Offset
        {
            get => _offset;
            set => _offset = value;
        }
        public Vector2 Size
        {
            get => _size;
            set => _size = value;
        }

        public Box(Vector2 position, Vector2 size)
        {
            _offset = position;
            _size = size;
        }

        public Vector2 RandomPointInBox()
        {
            float x = Random.Range(_offset.x - _size.x * 0.5f, _offset.x + _size.x * 0.5f);
            float y = Random.Range(_offset.y - _size.y * 0.5f, _offset.y + _size.y * 0.5f);
            return new Vector2(x, y);
        }
    }
}