using System;

namespace Asce.BehaviourTrees
{
    /// <summary> 
    ///     A node that always returns <see cref="NodeState.Running"/> while a timer is counting down. 
    /// </summary>
    public class WaitNode : DecoratorNode
    {
        private float _duration;
        private float _startTime = -1f;
        private bool _autoReset;

        public float Duration
        {
            get => _duration;
            set => _duration = value;
        }

        public float StartTime
        {
            get => _startTime;
            set => _startTime = value;
        }

        public bool AutoReset
        {
            get => _autoReset;
            set => _autoReset = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WaitNode"/> class.
        ///     <br/>
        ///     Waits for the specified duration before returning Success.
        /// </summary>
        /// <param name="duration"> Duration to wait in seconds. </param>
        public WaitNode(Node child, float duration, bool autoReset = true) : base(child)
        {
            _duration = duration;
            _autoReset = autoReset;
        }

        public override NodeState Tick()
        {
            if (StartTime < 0)
                StartTime = UnityEngine.Time.time;

            float elapsed = UnityEngine.Time.time - StartTime;
            if (elapsed < Duration)
                return NodeState.Running;

            if (AutoReset) this.Reset();
            if (Child == null) throw new InvalidOperationException("Child node is not set.");
            NodeState result = Child.Tick();
            return result;
        }

        public override void Reset()
        {
            base.Reset();
            _startTime = -1f;
        }
    }
}