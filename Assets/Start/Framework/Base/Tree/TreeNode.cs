using System.Collections.Generic;

namespace Start.Framework
{
    /// <summary>
    /// 多叉树节点
    /// </summary>
    /// <typeparam name="TNode">节点</typeparam>
    /// <typeparam name="TData">数据</typeparam>
    public abstract class TreeNode<TNode, TData> where TNode : TreeNode<TNode, TData>
    {
        /// <summary>
        /// 数据
        /// </summary>
        public TData Value { get; private set; }

        /// <summary>
        /// 父节点
        /// </summary>
        public TNode Parent { get; private set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<TNode> Children { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="value">value</param>
        public TreeNode(TData value)
        {
            Value = value;
            Children = new List<TNode>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="value">value</param>
        public TreeNode(TData value, TNode parent)
        {
            Value = value;
            Parent = parent;
            Children = new List<TNode>();
        }

        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="child">子节点</param>
        public void AddChild(TNode child)
        {
            child.SetParent((TNode)this);
            Children.Add(child);
        }

        /// <summary>
        /// 移除子阶段
        /// </summary>
        /// <param name="child"></param>
        public void RemoveChild(TNode child)
        {
            child.SetParent(null);
            Children.Remove(child);
        }

        /// <summary>
        /// 设置父节点
        /// </summary>
        /// <param name="parent"></param>
        public virtual void SetParent(TNode parent)
        {
            Parent = parent;
        }
    }
}