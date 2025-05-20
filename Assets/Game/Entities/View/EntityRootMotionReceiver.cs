using UnityEngine;

namespace Asce.Game.Entities
{
    public class EntityRootMotionReceiver : MonoBehaviour
    {
        private Animator _animator;

        private Vector2 _rootMotionVelocity;

        public Vector2 RootMotionVelocity => _rootMotionVelocity;


        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void OnAnimatorMove()
        {
            _rootMotionVelocity = _animator.deltaPosition / Time.deltaTime;
        }
    }
}