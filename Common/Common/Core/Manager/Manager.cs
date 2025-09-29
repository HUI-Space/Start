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
        private static readonly LinkedList<IManager> _mangerLinkedList = new LinkedList<IManager>();
        // 存储管理者实例的字典，通过类型快速查找
        private static readonly Dictionary<Type, IManager> _mangerDictionary = new Dictionary<Type, IManager>();
        // 存储需要更新的管理者的链表，按优先级排序
        private static readonly LinkedList<IUpdateManger> _updateMangerLinkedList = new LinkedList<IUpdateManger>();
        
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
            foreach (IManager manager in _mangerLinkedList)
            {
                await manager.Initialize();
                AddUpdateManger(manager);
            }
        }
        
        /// <summary>
        /// 更新所有需要更新的管理者
        /// </summary>
        /// <param name="elapseSeconds">自上次更新以来的时间</param>
        /// <param name="realElapseSeconds">实际经过的时间</param>
        public static void Update(float elapseSeconds, float realElapseSeconds)
        {
            // 遍历更新管理者链表，调用每个管理者的Update方法
            foreach (IUpdateManger manager in _updateMangerLinkedList)
            {
                manager.Update(elapseSeconds, realElapseSeconds);
            }
        }
    
        /// <summary>
        /// 反初始化所有管理者，清除资源
        /// </summary>
        public static async Task DeInitialize()
        {
            // 清空更新管理者链表
            _updateMangerLinkedList.Clear();
            // 反初始化所有管理者
            foreach (IManager manager in _mangerLinkedList)
            {
                await manager.DeInitialize();
            }
            // 清空管理者链表
            _mangerLinkedList.Clear();
        }
    
        /// <summary>
        /// 获取指定类型的管理者实例
        /// </summary>
        /// <typeparam name="T">管理者类型</typeparam>
        /// <returns>指定类型的管理者实例</returns>
        public static T GetManger<T>()where T :IManager
        {
            return (T)GetManger(typeof(T));
        }
        
        /// <summary>
        /// 获取指定类型的管理者实例
        /// </summary>
        /// <param name="type">管理者类型</param>
        /// <returns>指定类型的管理者实例</returns>
        private static IManager GetManger(Type type)
        {
            // 从字典中尝试获取管理者实例
            if (_mangerDictionary.TryGetValue(type, out IManager manager))
            {
                return manager;
            }
            return null;
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
    
            LinkedListNode<IManager> current = _mangerLinkedList.First;
            
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
                _mangerLinkedList.AddBefore(current, manager);
            }
            else
            {
                _mangerLinkedList.AddLast(manager);
            }
            // 添加管理者到字典
            _mangerDictionary.Add(type,manager);
        }
    
        /// <summary>
        /// 将管理者添加到更新管理者链表
        /// </summary>
        /// <param name="manager">管理者实例</param>
        private static void AddUpdateManger(IManager manager)
        {
            // 检查管理者是否需要更新
            if (manager is IUpdateManger updateManger)
            {
                LinkedListNode<IUpdateManger> updateManagerNode = _updateMangerLinkedList.First;
                // 根据优先级插入更新管理者到链表中
                while (updateManagerNode != null && updateManagerNode.Value is IManager up)
                {
                    if (manager.Priority < up.Priority)
                    {
                        break;
                    }
    
                    updateManagerNode = updateManagerNode.Next;
                }
    
                if (updateManagerNode != null)
                {
                    _updateMangerLinkedList.AddBefore(updateManagerNode, updateManger);
                }
                else
                {
                    _updateMangerLinkedList.AddLast(updateManger);
                }
            }
        }
    }
}