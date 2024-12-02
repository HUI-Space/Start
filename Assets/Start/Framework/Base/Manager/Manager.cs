using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Start.Framework
{
    public class Manager
    {
        private static readonly LinkedList<IManager> MangerLinkedList = new LinkedList<IManager>();
        private static readonly LinkedList<IUpdateManger> UpdateMangerLinkedList = new LinkedList<IUpdateManger>();
        private static readonly Dictionary<Type, IManager> MangerDictionary = new Dictionary<Type, IManager>();
        public static async Task Initialize()
        {
            var types = AssemblyUtility.GetTypes();
            foreach (Type type in types)
            {
                if (type.IsDefined(typeof(ManagerAttribute), false) && !type.IsAbstract)
                {
                    CreateManager(type);
                }
            }
            foreach (IManager manager in MangerLinkedList)
            {
                await manager.Initialize();
                AddUpdateManger(manager);
            }
        }
        
        public static void Update(float elapseSeconds, float realElapseSeconds)
        {
            foreach (IUpdateManger manager in UpdateMangerLinkedList)
            {
                manager.Update(elapseSeconds, realElapseSeconds);
            }
        }

        public static async Task DeInitialize()
        {
            UpdateMangerLinkedList.Clear();
            foreach (IManager manager in MangerLinkedList)
            {
                await manager.DeInitialize();
            }
            MangerLinkedList.Clear();
        }

        public static T GetManger<T>()where T :IManager
        {
            return (T)GetManger(typeof(T));
        }
        
        private static IManager GetManger(Type type)
        {
            if (MangerDictionary.TryGetValue(type, out IManager manager))
            {
                return manager;
            }
            return null;
        }
        
        private static void CreateManager(Type type)
        {
            IManager manager = (IManager)Activator.CreateInstance(type);
            if (manager == null)
            {
                return;
            }

            LinkedListNode<IManager> current = MangerLinkedList.First;
        
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
                MangerLinkedList.AddBefore(current, manager);
            }
            else
            {
                MangerLinkedList.AddLast(manager);
            }
            MangerDictionary.Add(type,manager);
        }

        private static void AddUpdateManger(IManager manager)
        {
            if (manager is IUpdateManger updateManger)
            {
                LinkedListNode<IUpdateManger> updateManagerNode = UpdateMangerLinkedList.First;
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
                    UpdateMangerLinkedList.AddBefore(updateManagerNode, updateManger);
                }
                else
                {
                    UpdateMangerLinkedList.AddLast(updateManger);
                }
            }
        }
    }
}