using System;
using System.Collections.Generic;

namespace Start
{
    public class Helper
    {
        /// <summary>
        /// 创建指定类型的子类型的实例(慎用原因是因为，框架需要)
        /// </summary>
        /// <typeparam name="T1">要创建实例的类型</typeparam>
        /// <returns>指定类型的子类型的实例，如果没有找到合适的子类型，则返回null</returns>
        internal static T1 CreateHelper<T1>()
        {
            // 获取所有子类型
            List<Type> types = AssemblyUtility.GetChildType(typeof(T1));
            Type temp = null;
            int childCount = 0;
            // 遍历所有子类型，寻找可以实例化的子类型
            foreach (Type type in types)
            {
                // 检查当前子类型是否为类并且不是抽象的，即可以实例化
                if (type.IsClass && !type.IsAbstract)
                {
                    temp = type;
                    childCount++;
                }
            }
            if (childCount == 0)
            {
                throw new Exception("没有找到合适的子类型:" + typeof(T1));
            }
            if (childCount > 1)
            {
                throw new Exception("找到多个合适的子类型:" + typeof(T1));
            }
            // 如果是，则创建该类型的实例并返回
            if (temp != null) 
                return (T1)Activator.CreateInstance(temp);
            return default;
        }
    }
}