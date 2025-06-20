using System;

namespace Asce.BehaviourTrees
{
    public class DataReceiverNode<T> : LeafNode
    {
        protected T _data;
        protected Action<T> _action;

        public T Data { set => _data = value; }

        public DataReceiverNode(Action<T> action = null)
        {
            _action = action;
        }

        public override NodeState Tick()
        {
            if (_action == null)
                throw new InvalidOperationException("Action is not set.");

            _action.Invoke(_data);
            return NodeState.Success;
        }
    }
}