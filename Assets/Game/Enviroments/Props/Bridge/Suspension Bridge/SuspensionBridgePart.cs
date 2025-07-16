using Asce.Managers;
using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Enviroments
{
    public class SuspensionBridgePart : GameComponent, IEnviromentComponent
    {
        [SerializeField, Readonly] protected Rigidbody2D _rigidbody;
        [SerializeField, Readonly] protected BoxCollider2D _collider;
        [SerializeField, Readonly] protected HingeJoint2D _hingeJoint;

        public Rigidbody2D Rigidbody => _rigidbody;
        public BoxCollider2D Collider => _collider;
        public HingeJoint2D HingeJoint => _hingeJoint;

        protected override void RefReset()
        {
            base.RefReset();
            this.LoadComponent(out _rigidbody);
            this.LoadComponent(out _collider);
            this.LoadComponent(out _hingeJoint);
        }
    }
}