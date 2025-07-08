using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TestUI : MonoBehaviour
{

    public void LoadAssetBundle(string filePath,Action<AssetBundle> callback)
    {
        AssetBundleCreateRequest assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(filePath);
        assetBundleCreateRequest.completed += (request) =>
        {
            AssetBundle bundle = assetBundleCreateRequest.assetBundle;
            callback?.Invoke(bundle);
        };
    }
    
    private async void Start()
    {
        try
        {
            // 普通协程转 Task
            await LongOperation().AsTask(this);
            Debug.Log("Long operation completed!");
            
            // 带返回值的协程
            int result = await GetResult().AsTask<int>(this);
            Debug.Log($"Result: {result}");
            
            // 带取消的协程
            var cts = new CancellationTokenSource(2000); // 2秒后自动取消
            await CancelableOperation().AsTask(this, cts.Token);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Operation was canceled");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error: {ex.Message}");
        }
    }
    
    // 普通协程re
    private IEnumerator LongOperation()
    {
        yield return new WaitForSeconds(1f);
    }
    
    // 带返回值的协程
    private IEnumerator GetResult()
    {
        yield return new WaitForSeconds(0.5f);
        yield return 42;
    }
    
    // 可取消的协程
    private IEnumerator CancelableOperation()
    {
        while (true)
        {
            Debug.Log("Working...");
            yield return new WaitForSeconds(0.5f);
        }
    }
}

public static class CoroutineExtensions
{
    // 将协程转换为 Task
    public static Task AsTask(this IEnumerator coroutine, MonoBehaviour owner)
    {
        var tcs = new TaskCompletionSource<object>();
        
        // 启动协程并监听完成状态
        owner.StartCoroutine(WrapCoroutine(coroutine, tcs));
        return tcs.Task;
    }
    
    // 带返回值的协程转换
    public static Task<T> AsTask<T>(this IEnumerator coroutine, MonoBehaviour owner)
    {
        var tcs = new TaskCompletionSource<T>();
        
        owner.StartCoroutine(WrapCoroutineWithResult(coroutine, tcs));
        return tcs.Task;
    }
    
    // 监听协程完成，设置 Task 结果
    private static IEnumerator WrapCoroutine(IEnumerator coroutine, TaskCompletionSource<object> tcs)
    {
        yield return coroutine;
        
        try
        {
            // 检查协程是否有返回值（通过 yield return）
            if (coroutine is IEnumerator<object> genericCoroutine)
            {
                tcs.SetResult(genericCoroutine.Current);
            }
            else
            {
                tcs.SetResult(null);
            }
        }
        catch (Exception ex)
        {
            tcs.SetException(ex);
        }
    }
    
    // 处理带返回值的协程
    private static IEnumerator WrapCoroutineWithResult<T>(IEnumerator coroutine, TaskCompletionSource<T> tcs)
    {
        yield return coroutine;
        
        try
        {
            // 尝试获取协程的返回值
            if (coroutine.Current is T result)
            {
                tcs.SetResult(result);
            }
            else
            {
                tcs.SetException(new InvalidOperationException($"Coroutine did not return a value of type {typeof(T)}"));
            }
        }
        catch (Exception ex)
        {
            tcs.SetException(ex);
        }
    }
    
    public static Task AsTask(this IEnumerator coroutine, MonoBehaviour owner, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<object>();

        // 注册取消回调
        if (cancellationToken.CanBeCanceled)
        {
            cancellationToken.Register(() =>
            {
                tcs.TrySetCanceled();
            });
        }

        owner.StartCoroutine(WrapCoroutineWithCancellation(coroutine, tcs, cancellationToken));
        return tcs.Task;
    }

    private static IEnumerator WrapCoroutineWithCancellation(IEnumerator coroutine, TaskCompletionSource<object> tcs, CancellationToken cancellationToken)
    {
        while (true)
        {
            // 检查是否取消
            if (cancellationToken.IsCancellationRequested)
            {
                tcs.TrySetCanceled();
                yield break;
            }

            // 推进协程
            bool moveNext;
            try
            {
                moveNext = coroutine.MoveNext();
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
                yield break;
            }

            if (!moveNext)
            {
                // 协程完成
                try
                {
                    tcs.TrySetResult(coroutine.Current);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
                yield break;
            }

            // 等待下一帧或嵌套的 yield
            yield return coroutine.Current;
        }
    }
}


