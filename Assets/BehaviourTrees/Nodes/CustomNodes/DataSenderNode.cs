using System;

namespace Asce.BehaviourTrees
{
    public class DataSenderNode<T> : LeafNode
    {
        protected T _data;
        protected Func<T> _provider;

        public T Data => _data;

        public DataSenderNode(Func<T> provider = null)
        {
            _provider = provider;
        }

        public override NodeState Tick()
        {
            if (_provider == null) 
                throw new InvalidOperationException("Provider function is not set.");

            _data = _provider.Invoke();
            return NodeState.Success;
        }

    }
}