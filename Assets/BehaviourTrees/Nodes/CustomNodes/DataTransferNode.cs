using System;
using System.Collections.Generic;

namespace Asce.BehaviourTrees
{
    /// <summary>
    ///     A behavior tree node that transfers data from a sender to a receiver.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of data being transferred.
    /// </typeparam>
    public class DataTransferNode<T> : Node
    {
        private DataSenderNode<T> _sender;
        private DataReceiverNode<T> _receiver;

        public DataSenderNode<T> Sender
        {
            get => _sender;
            set => _sender = value;
        }

        public DataReceiverNode<T> Receiver
        {
            get => _receiver;
            set => _receiver = value;
        }


        public DataTransferNode(DataSenderNode<T> sender, DataReceiverNode<T> receiver = null)
        {
            _sender = sender;
            _receiver = receiver;
        }

        public override NodeState Tick()
        {
            if (_sender == null)
                throw new InvalidOperationException("Sender is not set.");

            _sender.Tick();

            if (_receiver == null) return NodeState.Success;

            _receiver.Data = _sender.Data;
            return _receiver.Tick();
        }

        public override void Reset()
        {
            _sender?.Reset();
            _receiver?.Reset();
        }

        public override List<Node> GetChildren()
        {
            List<Node> children = new ();
            if (_sender != null) children.Add(_sender);
            if (_receiver != null) children.Add(_receiver);
            return children;
        }
    }

}