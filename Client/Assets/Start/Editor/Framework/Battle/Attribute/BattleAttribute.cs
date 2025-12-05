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
/// 仅读字段属性
/// </summary>
public class ReadOnlyAttribute : Attribute
{
}

/// <summary>
/// 拷贝属性
/// </summary>
public class CopyAttribute : System.Attribute
{
}

/// <summary>
/// 不可序列化字段
/// </summary>
public class NoSerializeAttribute : Attribute
{
}

/// <summary>
/// 序列化组件
/// </summary>
public class SerializeAttribute : Attribute
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


