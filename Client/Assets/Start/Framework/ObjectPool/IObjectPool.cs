using System;
using System.Collections.Generic;

namespace Start
{
    // 释放对象筛选器委托
    public delegate List<TObject> ReleaseObjectFilterCallback<TObject, TTarget>(List<TObject> candidateObjects, int toReleaseCount, DateTime expireTime) 
        where TObject : ObjectBase<TTarget>,new()
        where TTarget : class;
    
    public interface IObjectPool<TObject, TTarget> : IObjectPoolBase 
        where TObject : ObjectBase<TTarget>, new() 
        where TTarget : class
    {
        /// <summary>
        /// 获取对象池对象类型。
        /// </summary>
        Type ObjectType { get; }

        /// <summary>
        /// 获取或设置对象池的容量。
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// 获取对象池中对象的数量。
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 获取对象池中能被释放的对象的数量。
        /// </summary>
        int CanReleaseCount { get; }

        /// <summary>
        /// 获取是否允许对象被多次获取。
        /// </summary>
        bool AllowMultiSpawn { get; }

        /// <summary>
        /// 获取或设置对象池自动释放可释放对象的间隔秒数。
        /// </summary>
        float AutoReleaseInterval { get; }

        /// <summary>
        /// 获取或设置对象池对象过期秒数。
        /// </summary>
        float ExpireTime { get; }

        /// <summary>
        /// 初始化对象池
        /// </summary>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="allowMultiSpawn">允许多次获取</param>
        /// <param name="autoReleaseInterval">对象池自动释放可释放对象的间隔秒数</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        void Initialize(int capacity, bool allowMultiSpawn, float autoReleaseInterval, float expireTime, int priority);

        /// <summary>
        /// 创建对象。
        /// </summary>
        /// <param name="obj">对象。</param>
        /// <param name="spawned">对象是否已被获取。</param>
        void Register(TObject obj, bool spawned);

        /// <summary>
        /// 检查对象。
        /// </summary>
        /// <param name="name">对象名称。</param>
        /// <returns>要检查的对象是否存在。</returns>
        bool CanSpawn(string name);

        /// <summary>
        /// 获取对象。
        /// </summary>
        /// <returns>要获取的对象。</returns>
        TObject Spawn();

        /// <summary>
        /// 获取对象。
        /// </summary>
        /// <param name="name">要获取的对象的名称。</param>
        /// <returns>要获取的对象。</returns>
        TObject Spawn(string name);
        
        /// <summary>
        /// 回收对象。
        /// </summary>
        /// <param name="obj">要回收的对象。</param>
        void UnSpawn(TObject obj);

        /// <summary>
        /// 回收对象。
        /// </summary>
        /// <param name="target">要回收的对象。</param>
        void UnSpawn(TTarget target);

        /// <summary>
        /// 设置对象是否被加锁。
        /// </summary>
        /// <param name="obj">要设置被加锁的对象。</param>
        /// <param name="locked">是否被加锁。</param>
        void SetLocked(TObject obj, bool locked);

        /// <summary>
        /// 设置对象的优先级。
        /// </summary>
        /// <param name="obj">要设置优先级的对象。</param>
        /// <param name="priority">优先级。</param>
        void SetPriority(TObject obj, int priority);

        /// <summary>
        /// 释放对象。
        /// </summary>
        /// <param name="obj">要释放的对象。</param>
        /// <returns>释放对象是否成功。</returns>
        bool ReleaseObject(TObject obj);

        /// <summary>
        /// 释放对象池中的可释放对象。
        /// </summary>
        /// <param name="toReleaseCount">尝试释放对象数量。</param>
        /// <param name="releaseObjectFilterCallback">释放对象筛选函数。</param>
        void Release(int toReleaseCount, ReleaseObjectFilterCallback<TObject,TTarget> releaseObjectFilterCallback);
    }
}