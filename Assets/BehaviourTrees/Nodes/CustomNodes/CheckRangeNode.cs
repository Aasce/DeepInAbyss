using System;

namespace Asce.BehaviourTrees
{
    public class CheckRangeNode : LeafNode
    {
        private Func<float> _getRangeFunction;
        private float _range;

        public Func<float> GetRangeFunction
        {
            get => _getRangeFunction;
            set => _getRangeFunction = value;
        }

        public float Range
        {
            get => _range;
            set => _range = value;
        }

        public CheckRangeNode(Func<float> getRangeFunction, float range = 1f)
        {
            _getRangeFunction = getRangeFunction;
            _range = range;
        }

        public override NodeState Tick()
        {
            if (_getRangeFunction == null) throw new InvalidOperationException("GetRangeFunction is not set.");

            float currentRange = _getRangeFunction.Invoke();
            return currentRange <= _range ? NodeState.Success : NodeState.Failure;
        }
    }
}