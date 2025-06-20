using System.Collections.ObjectModel;

namespace Asce.BehaviourTrees
{
    public interface ICompositeNode
    {
        public ReadOnlyCollection<Node> Children { get; }
    }
}