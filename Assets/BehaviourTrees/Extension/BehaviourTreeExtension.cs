using System;
using System.Collections.Generic;

namespace Asce.BehaviourTrees
{
    public static class BehaviourTreeExtension
    {
        public static void BFS(this BehaviourTree tree, Action<Node> action)
        {
            if (tree == null) throw new ArgumentNullException("BehaviourTree or action cannot be null.");
            Queue<Node> queue = new ();
            queue.Enqueue(tree.RootNode);

            while (queue.Count > 0)
            {
                Node currentNode = queue.Dequeue();
                if (currentNode == null) continue;
                action?.Invoke(currentNode);

                foreach (Node child in currentNode.GetChildren())
                {
                    if (child != null) queue.Enqueue(child);
                }
            }
        }

        public static void DFS(this BehaviourTree tree, Action<Node> action)
        {
            if (tree == null) throw new ArgumentNullException("BehaviourTree or action cannot be null.");
            Stack<Node> stack = new();
            stack.Push(tree.RootNode);

            while (stack.Count > 0)
            {
                Node currentNode = stack.Pop();
                if (currentNode == null) continue;
                action?.Invoke(currentNode);

                List<Node> children = currentNode.GetChildren();
                children.Reverse(); // Reverse to maintain the order of children when popping from stack
                foreach (Node child in children)
                {
                    if (child != null) stack.Push(child);
                }
            }
        }
    }
}