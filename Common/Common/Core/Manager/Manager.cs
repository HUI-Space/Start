using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Start
{
    /// <summary>
    /// 管理者类，负责初始化、更新和反初始化各个管理者
    /// </summary>
    public class Manager
    {
        // 存储所有管理者的链表，按优先级排序
        private static readonly LinkedList<IManager> _managerLinkedList = new LinkedList<IManager>();
        // 存储管理者实例的字典，通过类型快速查找
        private static readonly Dictionary<Type, IManager> _managerDictionary = new Dictionary<Type, IManager>();
        // 存储需要更新的管理者的列表
        private static readonly List<IUpdateManager> _updateManagerList = new List<IUpdateManager>();
        
        /// <summary>
        /// 初始化所有管理者
        /// </summary>
        public static async Task Initialize()
        {
            Type type = typeof(IManager);
            // 获取所有IManager的子类
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach (Type t in types)
                {
                    if (type.IsAssignableFrom(t) && type != t && !t.IsAbstract)
                    {
                        // 创建管理者实例
                        CreateManager(t);
                    }
                }
            }
            // 初始化所有管理者，并添加到更新管理者链表
            foreach (IManager manager in _managerLinkedList)
            {
                await manager.Initialize();
            }
            
            foreach (IManager manager in _managerLinkedList)
            {
                if (manager is IUpdateManager updateManager)
                {
                    _updateManagerList.Add(updateManager);
                }
            }
        }
        
        /// <summary>
        /// 更新所有需要更新的管理者
        /// </summary>
        /// <param name="elapseSeconds">自上次更新以来的时间</param>
        /// <param name="realElapseSeconds">实际经过的时间</param>
        public static void Update(float elapseSeconds, float realElapseSeconds)
        {
            // 使用 for 循环零 GC
            for (int i = 0; i < _updateManagerList.Count; i++)
            {
                try
                {
                    _updateManagerList[i].Update(elapseSeconds, realElapseSeconds);
                }
                catch (Exception ex)
                {
                    throw new Exception($"UpdateManager {_updateManagerList[i].GetType().Name} update failed.", ex);
                }
            }
        }
    
        /// <summary>
        /// 反初始化所有管理者，清除资源
        /// </summary>
        public static async Task DeInitialize()
        {
            // 清空更新管理者链表
            _updateManagerList.Clear();
            // 反初始化所有管理者
            // 按优先级从高到低反初始化（与初始化顺序相反）
            var node = _managerLinkedList.Last;
            while (node != null)
            {
                await node.Value.DeInitialize();
                node = node.Previous;
            }
            // 清空管理者链表
            _managerLinkedList.Clear();
            _managerDictionary.Clear();
        }
    
        /// <summary>
        /// 获取指定类型的管理者实例
        /// </summary>
        /// <typeparam name="T">管理者类型</typeparam>
        /// <returns>指定类型的管理者实例</returns>
        public static T GetManager<T>()where T :IManager
        {
            return (T)GetManager(typeof(T));
        }
        
        /// <summary>
        /// 获取指定类型的管理者实例
        /// </summary>
        /// <param name="type">管理者类型</param>
        /// <returns>指定类型的管理者实例</returns>
        private static IManager GetManager(Type type)
        {
            // 从字典中尝试获取管理者实例
            if (_managerDictionary.TryGetValue(type, out IManager manager))
            {
                return manager;
            }
            throw new InvalidOperationException($"Manager of type {type.Name} not found.");
        }
        
        /// <summary>
        /// 创建管理者实例，并添加到管理者链表和字典
        /// </summary>
        /// <param name="type">管理者类型</param>
        private static void CreateManager(Type type)
        {
            // 创建管理者实例
            IManager manager = (IManager)Activator.CreateInstance(type);
            if (manager == null)
            {
                return;
            }
    
            LinkedListNode<IManager> current = _managerLinkedList.First;
            
            // 根据优先级插入管理者到链表中
            while (current != null)
            {
                if (manager.Priority < current.Value.Priority)
                {
                    break;
                }
    
                current = current.Next;
            }
    
            if (current != null)
            {
                _managerLinkedList.AddBefore(current, manager);
            }
            else
            {
                _managerLinkedList.AddLast(manager);
            }
            // 添加管理者到字典
            _managerDictionary.Add(type,manager);
        }
    }
}