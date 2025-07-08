using System;
using System.Collections.Generic;

namespace Start.Editor
{
    /// <summary>
    /// 默认值属性
    /// </summary>
    public class DefaultAttribute : Attribute
    {
        public string DefaultValue;

        public DefaultAttribute(string defaultValue)
        {
            DefaultValue = defaultValue;
        }
    }
    
    /// <summary>
    /// 代码注释
    /// </summary>
    public class CommentAttribute : Attribute
    {
        public string Comment;

        public CommentAttribute(string comment)
        {
            Comment = comment;
        }
    }

    /// <summary>
    /// 序列化组件
    /// </summary>
    public class SerializeComponentAttribute : Attribute
    {
    }

    /// <summary>
    /// 不可序列化字段
    /// </summary>
    public class NoneSerializeFiledAttribute : Attribute
    {
    }

    /// <summary>
    /// 仅读字段属性
    /// </summary>
    public class ReadOnlyFieldAttribute : Attribute
    {
    }

    /// <summary>
    /// 脏数据检测
    /// </summary>
    public class DirtyCheckAttribute : Attribute
    {

    }

    /// <summary>
    /// 脏数据不检测
    /// </summary>
    public class NoDirtyCheckAttribute : Attribute
    {

    }

    public class Component
    {
        public interface IComponent
        {
            
        }

        public class MatchRuntimePropertyComponent : IComponent
        {
            [Comment("ID")] 
            public int Id;

            [Comment("ID列表")] 
            public int[] Ids;

            [Comment("子ID列表")] 
            public List<int> ChildIds;
        }
    }
}