using System;
using System.Collections.Generic;

namespace Start.Framework
{
    /// <summary>
    /// 多叉树
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public abstract class Tree<TNode, TData> where TNode : TreeNode<TNode, TData>
    {
        public TNode Root { get; private set; }

        public Tree(TNode rootValue)
        {
            Root = rootValue;
        }

        /// <summary>
        /// 深度优先遍历
        /// </summary>
        /// <param name="node"></param>
        /// <param name="action"></param>
        public void DepthFirstSearch(TNode node, Action<TNode> action)
        {
            if (node == null)
                return;

            // 处理当前节点
            action(node);

            // 递归处理所有子节点
            foreach (var child in node.Children)
            {
                DepthFirstSearch(child, action);
            }
        }

        /// <summary>
        /// 广度优先遍历
        /// </summary>
        /// <param name="root"></param>
        /// <param name="action"></param>
        public void BreadthFirstSearch(TNode root, Action<TNode> action)
        {
            if (root == null)
                return;

            var queue = new Queue<TNode>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                // 处理当前节点
                action(current);
                foreach (var child in current.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }
    }
}