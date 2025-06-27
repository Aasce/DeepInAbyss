using System;
using UnityEngine;

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
            GetRangeFunction = getRangeFunction;
            Range = range;
        }

        public override NodeState Tick()
        {
            if (GetRangeFunction == null) throw new InvalidOperationException("GetRangeFunction is not set.");

            float currentRange = GetRangeFunction.Invoke();
            return currentRange <= Range ? NodeState.Success : NodeState.Failure;
        }
    }
}