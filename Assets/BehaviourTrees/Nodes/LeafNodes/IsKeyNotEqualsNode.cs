namespace Asce.BehaviourTrees
{
    /// <summary>
    ///     A leaf node that checks whether the value stored in the blackboard at the specified key
    ///     is NOT equal to the provided value.
    /// </summary>
    /// <typeparam name="T"> The type of value to compare. </typeparam>
    public class IsKeyNotEqualsNode<T> : LeafNode
    {
        private string _key;
        private T _value;

        /// <summary>
        ///     Key to retrieve the value from the blackboard.
        /// </summary>
        public string Key
        {
            get => _key;
            set => _key = value;
        }

        /// <summary>
        ///     The value to compare with the blackboard value.
        /// </summary>
        public T Value
        {
            get => _value;
            set => _value = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="IsKeyNotEqualsNode{T}"/> class.
        /// </summary>
        /// <param name="key"> The blackboard key to check. </param>
        /// <param name="value"> The value to compare against. </param>
        public IsKeyNotEqualsNode(string key, T value)
        {
            Key = key;
            Value = value;
        }

        /// <summary>
        ///     Compares the stored value with the blackboard value.
        /// </summary>
        /// <returns>
        ///     <see cref="NodeState.Success"/> if values are not equal;
        ///     <see cref="NodeState.Failure"/> if equal or if key not found.
        /// </returns>
        public override NodeState Tick()
        {
            if (!Tree.Blackboard.TryGet(Key, out T storedValue)) return NodeState.Failure;
            return object.Equals(Value, storedValue) ? NodeState.Failure : NodeState.Success;
        }
    }

}