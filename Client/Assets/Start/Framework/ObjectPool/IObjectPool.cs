using System;
using System.Collections.Generic;

namespace Start
{
    /// <summary>
    /// 对象池对象过滤委托：用于筛选待释放的对象
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="list">可释放对象列表</param>
    /// <param name="toReleaseCount">期望释放数量</param>
    /// <param name="currentTime">当前UTC时间</param>
    /// <returns>实际要释放的对象列表</returns>
    public delegate List<ObjectStatus<T>> ReleaseObjectFilterCallback<T>(
        List<ObjectStatus<T>> list, 
        int toReleaseCount, 
        DateTime currentTime) where T : class, IObject, new();
    
    /// <summary>
    /// 对象池基础接口
    /// </summary>
    public interface IObjectPool : IRecycle
    {
        /// <summary>
        /// 更新对象池（用于自动释放逻辑）
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝秒数</param>
        /// <param name="realElapseSeconds">真实流逝秒数</param>
        void Update(float elapseSeconds, float realElapseSeconds);
    }
    
    /// <summary>
    /// 泛型对象池接口
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    public interface IObjectPool<T> : IObjectPool where T : class, IObject, new()
    {
        /// <summary>
        /// 获取对象池管理的对象类型
        /// </summary>
        Type ObjectType { get; }

        /// <summary>
        /// 获取对象池容量（0表示无上限）
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// 获取/设置对象过期时间（秒）：超过该时间未使用的对象优先释放
        /// </summary>
        float ExpireTime { get; }
        
        /// <summary>
        /// 获取/设置自动释放间隔（秒）
        /// </summary>
        float AutoReleaseInterval { get; }
        
        /// <summary>
        /// 获取对象池中总对象数（使用中+空闲）
        /// </summary>
        int Count { get; }
        
        /// <summary>
        /// 获取对象池中可释放的空闲对象数
        /// </summary>
        int CanReleaseCount { get; }
        
        /// <summary>
        /// 从对象池获取对象
        /// </summary>
        /// <returns>对象实例</returns>
        T Spawn();
        
        /// <summary>
        /// 获取对象的状态信息
        /// </summary>
        /// <param name="t">对象实例</param>
        /// <returns>对象状态</returns>
        ObjectStatus<T> GetObjectStatus(T t);
        
        /// <summary>
        /// 回收对象到对象池
        /// </summary>
        /// <param name="t">对象实例</param>
        void UnSpawn(T t);

        /// <summary>
        /// 设置对象是否锁定（锁定后不可释放）
        /// </summary>
        /// <param name="t">对象实例</param>
        /// <param name="locked">是否锁定</param>
        void SetLocked(T t, bool locked);
        
        /// <summary>
        /// 设置对象优先级
        /// </summary>
        /// <param name="t">对象实例</param>
        /// <param name="priority">优先级（数值越低越优先保留）</param>
        void SetPriority(T t, int priority);
        
        /// <summary>
        /// 释放单个对象（从对象池中永久移除）
        /// </summary>
        /// <param name="t">对象实例</param>
        /// <returns>是否释放成功</returns>
        bool ReleaseObject(T t);

        /// <summary>
        /// 自动释放超出容量的可释放对象
        /// </summary>
        void Release();
        
        /// <summary>
        /// 自定义释放指定数量的对象
        /// </summary>
        /// <param name="toReleaseCount">期望释放数量</param>
        /// <param name="releaseObjectFilterCallback">释放筛选函数</param>
        void Release(int toReleaseCount, ReleaseObjectFilterCallback<T> releaseObjectFilterCallback);
    }
}