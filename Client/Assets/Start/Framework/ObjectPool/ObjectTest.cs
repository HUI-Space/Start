using System;
using Start;
using UnityEngine;

public class ObjectTest
{
    public void Run()
    {
        var bulletPool = ObjectPoolManager.Instance.CreateObjectPool<Bullet,AssetBundle>();             
    }
}

// 自定义对象类（例如：游戏中的子弹）
public class Bullet : ObjectBase<AssetBundle>
{

    private void Test()
    {
        
    }
}