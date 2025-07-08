using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using Start;

public class Test2 : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        await TestCustomAwaiter();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public async Task TestCustomAwaiter()
    {
        Debug.Log("开始");
    
        // 直接await整数（通过扩展方法）
        await 2000.Delay();
    
        Debug.Log("2秒后执行");
    
        // 也可以直接使用DelayAwaitable
        await new DelayAwaitable(1000);
    
        Debug.Log("又过了1秒");
    }
}


// 自定义可等待类型
public class DelayAwaitable
{
    private readonly int _milliseconds;

    public DelayAwaitable(int milliseconds)
    {
        _milliseconds = milliseconds;
    }

    // 返回等待器
    public DelayAwaiter GetAwaiter() => new DelayAwaiter(_milliseconds);
}

// 自定义等待器
public class DelayAwaiter : INotifyCompletion
{
    private readonly int _milliseconds;
    private Action _continuation;

    public DelayAwaiter(int milliseconds)
    {
        _milliseconds = milliseconds;
        IsCompleted = false;
    }

    // 是否已完成
    public bool IsCompleted { get; private set; }

    // 获取结果（这里返回void）
    public void GetResult() { }

    // 注册完成回调
    public void OnCompleted(Action continuation)
    {
        _continuation = continuation;
        
        // 使用Timer模拟延迟
        System.Threading.Timer timer = null;
        timer = new System.Threading.Timer(_ =>
        {
            IsCompleted = true;
            timer.Dispose();
            _continuation?.Invoke();
        }, null, _milliseconds, System.Threading.Timeout.Infinite);
    }
}

// 扩展方法，让int支持await
public static class IntExtension
{
    public static DelayAwaitable Delay(this int milliseconds)
    {
        return new DelayAwaitable(milliseconds);
    }
}