using System;

/// <summary>
/// 默认值属性
/// </summary>
public class DefaultAttribute : Attribute
{
    /// <summary>
    /// 默认值属性
    /// </summary>
    public string DefaultValue;
    
    /// <summary>
    /// 默认值属性
    /// </summary>
    /// <param name="defaultValue">默认值</param>
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
    /// <summary>
    /// 注释
    /// </summary>
    public string Comment;

    /// <summary>
    /// 代码注释
    /// </summary>
    /// <param name="comment">注释</param>
    public CommentAttribute(string comment)
    {
        Comment = comment;
    }
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

/// <summary>
/// 可复制容器成员类型属性
/// </summary>
public class CopyableContainerMemberTypeAttribute : System.Attribute
{
}
/// <summary>
/// 序列化组件
/// </summary>
public class SerializeComponentAttribute : Attribute
{
}