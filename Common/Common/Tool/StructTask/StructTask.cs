using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Start
{
    /// <summary>
    /// 轻量级可等待任务结构体
    /// </summary>
    /// <remarks>
    /// <para>这是一个零GC的轻量级异步任务实现，设计用于替代Task在某些高性能场景下的使用。</para>
    /// <para><b>核心特性：</b></para>
    /// <list type="bullet">
    ///   <item><description>结构体实现，避免堆分配和GC压力</description></item>
    ///   <item><description>支持async/await语法糖</description></item>
    ///   <item><description>内置版本号机制防止ABA问题</description></item>
    ///   <item><description>使用SpinLock实现无锁同步</description></item>
    ///   <item><description>支持CancellationToken取消</description></item>
    ///   <item><description>支持与Task/ValueTask互操作</description></item>
    /// </list>
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// // 创建任务
    /// var task = StructTask.Create();
    /// // 在其他地方设置结果
    /// task.SetResult();
    /// // 等待任务完成
    /// await task;
    /// </code>
    /// </remarks>
    public readonly struct StructTask : ICriticalNotifyCompletion
    {
        /// <summary>
        /// 全局回调队列单例
        /// </summary>
        /// <remarks>
        /// 所有任务共享同一个回调队列实例，用于管理任务状态和回调执行。
        /// 使用单例模式避免静态构造器的性能开销。
        /// </remarks>
        private static readonly CallbackQueue _callbackQueue = CallbackQueue.Instance;

        /// <summary>
        /// 已完成的空任务单例
        /// </summary>
        /// <remarks>
        /// 预先创建的已完成任务，用于FromResult()等场景，避免重复创建。
        /// 这是一个零分配的操作，直接返回静态单例。
        /// </remarks>
        public static readonly StructTask CompletedTask = CreateCompleted();

        /// <summary>
        /// 任务唯一标识符
        /// </summary>
        /// <remarks>
        /// 使用short类型（16位）作为ID，范围1-32767。
        /// ID=0表示未初始化的任务。
        /// ID用于在CallbackQueue中索引任务状态。
        /// </remarks>
        private readonly short _id;

        /// <summary>
        /// 任务版本号
        /// </summary>
        /// <remarks>
        /// 用于解决ABA问题。当ID被回收重用时，版本号会递增。
        /// 版本号与ID组合可以唯一标识一个任务生命周期。
        /// 版本号范围1-32767，溢出后从1重新开始。
        /// </remarks>
        private readonly short _version;

        /// <summary>
        /// 任务配置
        /// </summary>
        /// <remarks>
        /// 包含任务的配置选项，如是否捕获同步上下文等。
        /// 使用结构体避免额外的堆分配。
        /// </remarks>
        private readonly StructTaskConfig _config;

        /// <summary>
        /// 获取任务是否已完成
        /// </summary>
        /// <value>如果任务已完成（成功、失败或取消）则为true，否则为false</value>
        /// <remarks>
        /// 已完成状态包括：成功(Succeeded)、失败(Faulted)、取消(Canceled)。
        /// 此属性是非阻塞的，可以安全地在任何线程调用。
        /// </remarks>
        public bool IsCompleted => _callbackQueue.IsCompleted(_id, _version);

        /// <summary>
        /// 获取任务是否成功完成
        /// </summary>
        /// <value>如果任务成功完成则为true，否则为false</value>
        /// <remarks>
        /// 仅当任务通过SetResult()成功完成时返回true。
        /// 失败或取消的任务此属性为false。
        /// </remarks>
        public bool IsCompletedSuccessfully => _callbackQueue.IsCompletedSuccessfully(_id, _version);

        /// <summary>
        /// 获取任务是否失败
        /// </summary>
        /// <value>如果任务因异常而失败则为true，否则为false</value>
        /// <remarks>
        /// 当通过SetException()设置异常后，此属性为true。
        /// 调用GetResult()时会重新抛出该异常。
        /// </remarks>
        public bool IsFaulted => _callbackQueue.IsFaulted(_id, _version);

        /// <summary>
        /// 获取任务是否已取消
        /// </summary>
        /// <value>如果任务已被取消则为true，否则为false</value>
        /// <remarks>
        /// 当通过SetCanceled()取消任务后，此属性为true。
        /// 调用GetResult()时会抛出OperationCanceledException。
        /// </remarks>
        public bool IsCanceled => _callbackQueue.IsCanceled(_id, _version);

        /// <summary>
        /// 获取任务是否有效
        /// </summary>
        /// <value>如果任务已初始化（ID不为0）则为true，否则为false</value>
        /// <remarks>
        /// ID=0表示未初始化的任务（default值）。
        /// 用于检查结构体是否已正确创建。
        /// </remarks>
        public bool IsValid => _id != 0;

        /// <summary>
        /// 私有构造函数
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <param name="version">任务版本号</param>
        /// <param name="config">任务配置</param>
        /// <remarks>
        /// 私有构造确保任务只能通过Create()方法创建，
        /// 保证ID和版本号的有效性。
        /// </remarks>
        private StructTask(short id, short version, StructTaskConfig config = default)
        {
            _id = id;
            _version = version;
            _config = config;
        }

        /// <summary>
        /// 构造函数，创建新的StructTask实例
        /// </summary>
        /// <param name="manualRelease">是否手动释放，默认false（自动释放）</param>
        /// <param name="cancellationToken">可选的取消令牌</param>
        /// <remarks>
        /// <para>便捷构造函数，内部调用Create方法。</para>
        /// <para>使用示例：</para>
        /// <code>
        /// StructTask task = new StructTask();
        /// // 等同于 StructTask task = StructTask.Create();
        /// </code>
        /// </remarks>
        public StructTask(bool manualRelease = false,CancellationToken cancellationToken = default)
        {
            this = Create(manualRelease, cancellationToken);
        }

        /// <summary>
        /// 创建已完成的任务单例
        /// </summary>
        /// <returns>已完成的StructTask实例</returns>
        /// <remarks>
        /// 静态初始化时调用，创建一个预先完成的任务。
        /// 用于CompletedTask静态字段。
        /// </remarks>
        private static StructTask CreateCompleted()
        {
            StructTask task = Create();
            task.SetResult();
            return task;
        }

        /// <summary>
        /// 创建新的StructTask实例
        /// </summary>
        /// <param name="manualRelease">是否手动释放，默认false（自动释放）</param>
        /// <param name="cancellationToken">可选的取消令牌</param>
        /// <returns>新创建的StructTask实例</returns>
        /// <remarks>
        /// <para>这是创建任务的主要方法。</para>
        /// <para>创建过程：</para>
        /// <list type="number">
        ///   <item><description>从CallbackQueue分配ID和版本号</description></item>
        ///   <item><description>如果提供了可取消的CancellationToken，注册取消回调</description></item>
        ///   <item><description>返回新的StructTask实例</description></item>
        /// </list>
        /// <para><b>注意：</b>任务创建后处于Pending状态，需要调用SetResult/SetException/SetCanceled来完成。</para>
        /// <para><b>释放模式：</b></para>
        /// <list type="bullet">
        ///   <item><description>自动释放（默认）：第一次await后自动释放资源</description></item>
        ///   <item><description>手动释放：需要调用Release()方法手动释放资源，支持多次await</description></item>
        /// </list>
        /// </remarks>
        /// <exception cref="InvalidOperationException">当并发任务数量超过最大限制时抛出</exception>
        public static StructTask Create(bool manualRelease = false,CancellationToken cancellationToken = default)
        {
            TaskData taskData = _callbackQueue.Allocate(manualRelease);
            StructTask task = new StructTask(taskData.Id, taskData.Version);
            if (cancellationToken.CanBeCanceled)
            {
                CancellationTokenRegistration registration = cancellationToken.Register(() => task.SetCanceled());
                _callbackQueue.SetCancellationRegistration(taskData.Id, taskData.Version, registration);
            }

            return task;
        }

        /// <summary>
        /// 返回已完成的任务
        /// </summary>
        /// <returns>已完成的StructTask单例</returns>
        /// <remarks>
        /// 零分配操作，直接返回预创建的CompletedTask单例。
        /// 适用于需要返回void结果的异步方法。
        /// </remarks>
        public static StructTask FromResult()
        {
            return CompletedTask;
        }

        /// <summary>
        /// 创建已失败的任务
        /// </summary>
        /// <param name="exception">导致任务失败的异常</param>
        /// <returns>已失败的StructTask实例</returns>
        /// <remarks>
        /// 创建一个已经处于Faulted状态的任务。
        /// await此任务时会重新抛出异常。
        /// </remarks>
        /// <exception cref="ArgumentNullException">当exception为null时抛出</exception>
        public static StructTask FromException(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            StructTask task = Create();
            task.SetException(exception);
            return task;
        }

        /// <summary>
        /// 创建已取消的任务
        /// </summary>
        /// <param name="cancellationToken">可选的取消令牌</param>
        /// <returns>已取消的StructTask实例</returns>
        /// <remarks>
        /// 创建一个已经处于Canceled状态的任务。
        /// await此任务时会抛出OperationCanceledException。
        /// </remarks>
        public static StructTask FromCanceled(CancellationToken cancellationToken = default)
        {
            StructTask task = Create();
            task.SetCanceled();
            return task;
        }

        /// <summary>
        /// 从Task创建StructTask
        /// </summary>
        /// <param name="task">源Task实例</param>
        /// <returns>对应的StructTask实例</returns>
        /// <remarks>
        /// <para>将Task转换为StructTask，实现与现有异步代码的互操作。</para>
        /// <para>转换逻辑：</para>
        /// <list type="bullet">
        ///   <item><description>如果Task已完成：直接返回对应状态的StructTask</description></item>
        ///   <item><description>如果Task未完成：注册延续回调，Task完成时设置StructTask结果</description></item>
        /// </list>
        /// </remarks>
        /// <exception cref="ArgumentNullException">当task为null时抛出</exception>
        public static StructTask FromTask(Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (task.IsCompleted)
            {
                if (task.IsFaulted)
                {
                    return FromException(task.Exception?.InnerException ?? new Exception("Task faulted"));
                }

                if (task.IsCanceled)
                {
                    return FromCanceled();
                }

                return CompletedTask;
            }

            StructTask result = Create();
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    result.SetException(t.Exception?.InnerException ?? new Exception("Task faulted"));
                }
                else if (t.IsCanceled)
                {
                    result.SetCanceled();
                }
                else
                {
                    result.SetResult();
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
            return result;
        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        /// <summary>
        /// 从ValueTask创建StructTask
        /// </summary>
        /// <param name="valueTask">源ValueTask实例</param>
        /// <returns>对应的StructTask实例</returns>
        /// <remarks>
        /// <para>仅适用于.NET Standard 2.1及以上版本。</para>
        /// <para>将ValueTask转换为StructTask，实现与ValueTask的互操作。</para>
        /// <para>如果ValueTask已完成，直接返回对应状态；否则将其转换为Task后处理。</para>
        /// </remarks>
        public static StructTask FromValueTask(ValueTask valueTask)
        {
            if (valueTask.IsCompleted)
            {
                if (valueTask.IsFaulted)
                {
                    try { valueTask.GetAwaiter().GetResult(); }
                    catch (Exception ex) { return FromException(ex); }
                }
                if (valueTask.IsCanceled)
                {
                    return FromCanceled();
                }
                return CompletedTask;
            }
            StructTask result = Create();
            valueTask.AsTask().ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    result.SetException(t.Exception?.InnerException ?? new Exception("Task faulted"));
                }
                else if (t.IsCanceled)
                {
                    result.SetCanceled();
                }
                else
                {
                    result.SetResult();
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
            return result;
        }
#endif

        /// <summary>
        /// 将StructTask转换为Task
        /// </summary>
        /// <returns>对应的Task实例</returns>
        /// <remarks>
        /// <para>将StructTask转换为Task，实现与需要Task的API互操作。</para>
        /// <para>性能优化：</para>
        /// <list type="bullet">
        ///   <item><description>已完成任务：零分配，使用Task.CompletedTask/FromException/FromCanceled</description></item>
        ///   <item><description>未完成任务：分配TaskCompletionSource，任务完成时设置结果</description></item>
        /// </list>
        /// </remarks>
        public Task AsTask()
        {
            StructTask self = this;
            if (self.IsCompleted)
            {
                if (self.IsFaulted)
                {
                    try
                    {
                        self.GetResult();
                    }
                    catch (Exception ex)
                    {
                        return Task.FromException(ex);
                    }
                }

                if (self.IsCanceled)
                {
                    return Task.FromCanceled(new CancellationToken(self.IsCanceled));
                }

                return Task.CompletedTask;
            }

            TaskCompletionSource<object> tcs2 = new TaskCompletionSource<object>();
            self.OnCompleted(() =>
            {
                if (self.IsFaulted)
                {
                    try
                    {
                        self.GetResult();
                    }
                    catch (Exception ex)
                    {
                        tcs2.SetException(ex);
                    }
                }
                else if (self.IsCanceled)
                {
                    tcs2.SetCanceled();
                }
                else
                {
                    tcs2.SetResult(null);
                }
            }, allowDuplicate: true);
            return tcs2.Task;
        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        /// <summary>
        /// 将StructTask转换为ValueTask
        /// </summary>
        /// <returns>对应的ValueTask实例</returns>
        /// <remarks>
        /// 仅适用于.NET Standard 2.1及以上版本。
        /// 通过AsTask()转换后包装为ValueTask。
        /// </remarks>
        public ValueTask AsValueTask()
        {
            return new ValueTask(AsTask());
        }
#endif

        /// <summary>
        /// 配置await行为
        /// </summary>
        /// <param name="continueOnCapturedContext">
        /// true：捕获当前同步上下文，回调在捕获的上下文执行；
        /// false：不捕获上下文，回调在线程池执行
        /// </param>
        /// <returns>配置后的等待器</returns>
        /// <remarks>
        /// <para>类似于Task.ConfigureAwait()的功能。</para>
        /// <para>使用场景：</para>
        /// <list type="bullet">
        ///   <item><description>库代码中通常使用ConfigureAwait(false)避免死锁</description></item>
        ///   <item><description>UI代码中使用ConfigureAwait(true)确保回到UI线程</description></item>
        /// </list>
        /// </remarks>
        public ConfiguredStructTask ConfigureAwait(bool continueOnCapturedContext)
        {
            return new ConfiguredStructTask(this, continueOnCapturedContext);
        }

        /// <summary>
        /// 获取等待器（支持await语法）
        /// </summary>
        /// <returns>当前StructTask实例</returns>
        /// <remarks>
        /// 实现await模式必需的方法。
        /// 编译器会将 await task 转换为 task.GetAwaiter()。
        /// </remarks>
        public StructTask GetAwaiter()
        {
            return this;
        }

        /// <summary>
        /// 获取任务结果（阻塞等待）
        /// </summary>
        /// <remarks>
        /// <para>实现await模式必需的方法。</para>
        /// <para>行为：</para>
        /// <list type="bullet">
        ///   <item><description>成功：直接返回</description></item>
        ///   <item><description>失败：重新抛出原始异常，保留堆栈跟踪</description></item>
        ///   <item><description>取消：抛出OperationCanceledException</description></item>
        ///   <item><description>待定：抛出InvalidOperationException</description></item>
        /// </list>
        /// <para><b>释放行为：</b></para>
        /// <list type="bullet">
        ///   <item><description>自动释放模式（默认）：第一次调用后自动释放资源</description></item>
        ///   <item><description>手动释放模式：不会自动释放，需要调用Release()手动释放</description></item>
        /// </list>
        /// </remarks>
        /// <exception cref="InvalidOperationException">任务未初始化或仍在待定状态</exception>
        /// <exception cref="Exception">任务失败时重新抛出原始异常</exception>
        /// <exception cref="OperationCanceledException">任务被取消时抛出</exception>
        public void GetResult()
        {
            if (_id == 0)
            {
                throw new InvalidOperationException("StructTask is not initialized");
            }

            _callbackQueue.GetResult(_id, _version);
        }

        /// <summary>
        /// 注册完成回调
        /// </summary>
        /// <param name="continuation">任务完成时执行的回调</param>
        /// <remarks>
        /// <para>实现await模式必需的方法。</para>
        /// <para>如果任务已完成，回调会被立即执行；否则回调会被存储，任务完成时执行。</para>
        /// <para><b>注意：</b>默认情况下不允许重复注册回调，会抛出异常。</para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">任务未初始化或回调已注册</exception>
        public void OnCompleted(Action continuation)
        {
            if (_id == 0)
            {
                throw new InvalidOperationException("StructTask is not initialized");
            }

            _callbackQueue.OnCompleted(_id, _version, continuation, allowDuplicate: false,
                _config.ContinueOnCapturedContext);
        }

        /// <summary>
        /// 注册完成回调（内部方法，允许重复注册）
        /// </summary>
        /// <param name="continuation">任务完成时执行的回调</param>
        /// <param name="allowDuplicate">是否允许重复注册回调</param>
        /// <param name="continueOnCapturedContext">是否捕获同步上下文</param>
        /// <remarks>
        /// 内部使用的方法，支持更灵活的回调注册选项。
        /// WhenAny/WhenAll等组合操作使用此方法。
        /// </remarks>
        internal void OnCompleted(Action continuation, bool allowDuplicate, bool continueOnCapturedContext = true)
        {
            if (_id == 0)
            {
                throw new InvalidOperationException("StructTask is not initialized");
            }

            _callbackQueue.OnCompleted(_id, _version, continuation, allowDuplicate, continueOnCapturedContext);
        }

        /// <summary>
        /// 不安全注册完成回调
        /// </summary>
        /// <param name="continuation">任务完成时执行的回调</param>
        /// <remarks>
        /// <para>实现ICriticalNotifyCompletion接口的方法。</para>
        /// <para>与OnCompleted的区别：不捕获执行上下文，性能更高但安全性更低。</para>
        /// <para>在静态上下文中使用，编译器生成的状态机会选择合适的方法。</para>
        /// </remarks>
        public void UnsafeOnCompleted(Action continuation)
        {
            OnCompleted(continuation);
        }

        /// <summary>
        /// 设置任务成功完成
        /// </summary>
        /// <remarks>
        /// <para>将任务状态从Pending转换为Succeeded。</para>
        /// <para>调用此方法后，所有等待的回调会被调度执行。</para>
        /// <para><b>注意：</b>每个任务只能调用一次SetResult/SetException/SetCanceled。</para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">任务未初始化或已完成</exception>
        public void SetResult()
        {
            if (_id == 0)
            {
                throw new InvalidOperationException("StructTask is not initialized");
            }

            _callbackQueue.SetResult(_id, _version);
        }

        /// <summary>
        /// 设置任务失败
        /// </summary>
        /// <param name="exception">导致失败的异常</param>
        /// <remarks>
        /// <para>将任务状态从Pending转换为Faulted。</para>
        /// <para>调用此方法后，所有等待的回调会被调度执行。</para>
        /// <para>await此任务时，异常会被重新抛出，保留原始堆栈跟踪。</para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">任务未初始化或已完成</exception>
        /// <exception cref="ArgumentNullException">exception为null</exception>
        public void SetException(Exception exception)
        {
            if (_id == 0)
            {
                throw new InvalidOperationException("StructTask is not initialized");
            }

            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            _callbackQueue.SetException(_id, _version, exception);
        }

        /// <summary>
        /// 设置任务取消
        /// </summary>
        /// <remarks>
        /// <para>将任务状态从Pending转换为Canceled。</para>
        /// <para>调用此方法后，所有等待的回调会被调度执行。</para>
        /// <para>await此任务时，会抛出OperationCanceledException。</para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">任务未初始化或已完成</exception>
        public void SetCanceled()
        {
            if (_id == 0)
            {
                throw new InvalidOperationException("StructTask is not initialized");
            }

            _callbackQueue.SetCanceled(_id, _version);
        }

        /// <summary>
        /// 释放任务资源
        /// </summary>
        /// <remarks>
        /// <para>释放任务占用的资源，将ID归还到空闲池。</para>
        /// <para><b>注意：</b>调用此方法后，任务将无法再被await或访问。</para>
        /// <para>如果任务需要被多次await，可以在最后一次使用完毕后调用此方法。</para>
        /// </remarks>
        public void Release()
        {
            if (_id == 0)
            {
                return;
            }

            _callbackQueue.Release(_id, _version);
        }

        /// <summary>
        /// 任务完成后执行延续操作
        /// </summary>
        /// <param name="continuation">延续操作，接收完成的任务作为参数</param>
        /// <returns>代表延续操作的新任务</returns>
        /// <remarks>
        /// <para>类似于Task.ContinueWith的功能。</para>
        /// <para>延续操作会在原任务完成后执行，无论成功、失败还是取消。</para>
        /// <para>如果延续操作抛出异常，返回的任务会处于失败状态。</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">continuation为null</exception>
        public StructTask ContinueWith(Action<StructTask> continuation)
        {
            if (continuation == null)
            {
                throw new ArgumentNullException(nameof(continuation));
            }

            StructTask self = this;
            StructTask result = Create();
            OnCompleted(() =>
            {
                try
                {
                    continuation(self);
                    result.SetResult();
                }
                catch (Exception ex)
                {
                    result.SetException(ex);
                }
            }, allowDuplicate: true);
            return result;
        }

        /// <summary>
        /// 任务完成后执行延续操作并返回结果
        /// </summary>
        /// <typeparam name="T">延续操作的返回类型</typeparam>
        /// <param name="continuation">延续操作，接收完成的任务并返回结果</param>
        /// <returns>代表延续操作结果的新任务</returns>
        /// <remarks>
        /// <para>类似于Task&lt;TResult&gt;.ContinueWith的功能。</para>
        /// <para>延续操作的返回值会成为新任务的结果。</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">continuation为null</exception>
        public StructTask<T> ContinueWith<T>(Func<StructTask, T> continuation)
        {
            if (continuation == null)
            {
                throw new ArgumentNullException(nameof(continuation));
            }

            StructTask self = this;
            StructTask<T> result = StructTask<T>.Create();
            OnCompleted(() =>
            {
                try
                {
                    T value = continuation(self);
                    result.SetResult(value);
                }
                catch (Exception ex)
                {
                    result.SetException(ex);
                }
            }, allowDuplicate: true);
            return result;
        }

        /// <summary>
        /// 等待任意一个任务完成
        /// </summary>
        /// <param name="tasks">要等待的任务数组</param>
        /// <returns>当任意任务完成时完成的任务</returns>
        /// <remarks>
        /// <para>类似于Task.WhenAny的功能。</para>
        /// <para>快速路径：如果已有任务完成，立即返回。</para>
        /// <para>慢速路径：注册所有任务的回调，第一个完成的任务触发结果。</para>
        /// <para><b>注意：</b>返回的任务不包含哪个任务完成的信息。</para>
        /// </remarks>
        /// <exception cref="ArgumentException">tasks为null或空数组</exception>
        public static StructTask WhenAny(params StructTask[] tasks)
        {
            if (tasks == null || tasks.Length == 0)
            {
                throw new ArgumentException("Tasks cannot be null or empty", nameof(tasks));
            }

            for (int i = 0; i < tasks.Length; i++)
            {
                if (tasks[i].IsCompleted)
                {
                    return FromResult();
                }
            }

            StructTask result = Create();
            WhenAnyState state = RecyclablePool.Acquire<WhenAnyState>();
            state.ResultTask = result;
            for (int i = 0; i < tasks.Length; i++)
            {
                StructTask task = tasks[i];
                WhenAnyContinuation continuation = new WhenAnyContinuation(state, task);
                task.OnCompleted(continuation.Execute, allowDuplicate: true);
            }

            return result;
        }

        /// <summary>
        /// 等待所有任务完成
        /// </summary>
        /// <param name="tasks">要等待的任务数组</param>
        /// <returns>当所有任务完成时完成的任务</returns>
        /// <remarks>
        /// <para>类似于Task.WhenAll的功能。</para>
        /// <para>快速路径：如果所有任务已完成，立即返回。</para>
        /// <para>异常处理：</para>
        /// <list type="bullet">
        ///   <item><description>如果有任务失败，返回AggregateException包含所有异常</description></item>
        ///   <item><description>如果有任务取消（无异常），返回取消状态</description></item>
        ///   <item><description>如果全部成功，返回成功</description></item>
        /// </list>
        /// </remarks>
        /// <exception cref="ArgumentException">tasks为null</exception>
        public static StructTask WhenAll(params StructTask[] tasks)
        {
            if (tasks == null || tasks.Length == 0)
            {
                return CompletedTask;
            }

            int pendingCount = 0;
            List<Exception> exceptions = null;
            bool anyCanceled = false;
            for (int i = 0; i < tasks.Length; i++)
            {
                if (!tasks[i].IsCompleted)
                {
                    pendingCount++;
                }
                else if (tasks[i].IsFaulted)
                {
                    exceptions = exceptions ?? new List<Exception>();
                    try
                    {
                        tasks[i].GetResult();
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }
                else if (tasks[i].IsCanceled)
                {
                    anyCanceled = true;
                }
            }

            if (pendingCount == 0)
            {
                if (exceptions != null)
                {
                    return FromException(new AggregateException(exceptions));
                }

                if (anyCanceled)
                {
                    return FromCanceled();
                }

                return CompletedTask;
            }

            StructTask result = Create();
            WhenAllState state = RecyclablePool.Acquire<WhenAllState>();
            state.ResultTask = result;
            state.Remaining = pendingCount;
            state.Exceptions = exceptions;
            state.IsCanceled = anyCanceled;
            for (int i = 0; i < tasks.Length; i++)
            {
                if (!tasks[i].IsCompleted)
                {
                    StructTask task = tasks[i];
                    WhenAllContinuation continuation = new WhenAllContinuation(state, task);
                    task.OnCompleted(continuation.Execute, allowDuplicate: true);
                }
            }

            return result;
        }

        /// <summary>
        /// 创建延迟任务
        /// </summary>
        /// <param name="milliseconds">延迟的毫秒数</param>
        /// <param name="cancellationToken">可选的取消令牌</param>
        /// <returns>在指定时间后完成的任务</returns>
        /// <remarks>
        /// <para>类似于Task.Delay的功能。</para>
        /// <para>如果milliseconds小于等于0，立即返回完成的任务。</para>
        /// <para>使用Timer实现延迟，Timer在回调后立即释放。</para>
        /// <para>如果CancellationToken被取消，任务会进入取消状态。</para>
        /// </remarks>
        public static StructTask Delay(int milliseconds, CancellationToken cancellationToken = default)
        {
            StructTask task = Create(false,cancellationToken);
            if (milliseconds <= 0)
            {
                task.SetResult();
                return task;
            }

            DelayTimerState timerState = RecyclablePool.Acquire<DelayTimerState>();
            timerState.Task = task;
            Timer timer = new Timer(DelayTimerCallback, timerState, milliseconds, Timeout.Infinite);
            timerState.Timer = timer;
            return task;
        }

        /// <summary>
        /// Timer回调方法
        /// </summary>
        /// <param name="state">DelayTimerState实例</param>
        private static void DelayTimerCallback(object state)
        {
            DelayTimerState timerState = (DelayTimerState)state;
            timerState.Timer?.Dispose();
            timerState.Timer = null;
            timerState.Task.SetResult();
            RecyclablePool.Recycle(timerState);
        }

        /// <summary>
        /// 延迟任务的Timer状态
        /// </summary>
        /// <remarks>
        /// 用于持有Timer引用，以便在回调后释放Timer资源。
        /// 避免Timer泄漏。
        /// </remarks>
        private class DelayTimerState : IRecycle
        {
            public StructTask Task;
            public Timer Timer;

            public DelayTimerState()
            {
            }

            public DelayTimerState(StructTask task)
            {
                Task = task;
            }

            public void Recycle()
            {
                Timer?.Dispose();
                Timer = null;
                Task = default;
            }
        }

        /// <summary>
        /// 等待任务完成或超时
        /// </summary>
        /// <param name="task">要等待的任务</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>原任务完成或超时取消的任务</returns>
        /// <remarks>
        /// <para>如果任务在超时前完成，返回原任务的结果。</para>
        /// <para>如果超时，返回的任务会被取消。</para>
        /// </remarks>
        public static StructTask WaitAsync(StructTask task, TimeSpan timeout)
        {
            return WaitAsync(task, (int)timeout.TotalMilliseconds);
        }

        /// <summary>
        /// 等待任务完成或超时
        /// </summary>
        /// <param name="task">要等待的任务</param>
        /// <param name="milliseconds">超时毫秒数</param>
        /// <returns>原任务完成或超时取消的任务</returns>
        /// <remarks>
        /// <para>快速路径：如果任务已完成，直接返回。</para>
        /// <para>慢速路径：同时设置超时Timer和任务回调，先触发的生效。</para>
        /// <para>线程安全：使用锁确保只有一个结果被设置。</para>
        /// </remarks>
        public static StructTask WaitAsync(StructTask task, int milliseconds)
        {
            if (task.IsCompleted)
            {
                return task;
            }

            if (milliseconds <= 0)
            {
                return FromCanceled();
            }

            StructTask result = Create();
            WaitAsyncState timeoutState = RecyclablePool.Acquire<WaitAsyncState>();
            timeoutState.ResultTask = result;
            timeoutState.DelayTimer = new Timer(WaitAsyncTimeoutCallback, timeoutState, milliseconds, Timeout.Infinite);
            task.OnCompleted(() =>
            {
                lock (timeoutState)
                {
                    if (timeoutState.Completed)
                    {
                        return;
                    }

                    timeoutState.Completed = true;
                    timeoutState.DelayTimer?.Dispose();
                }

                if (task.IsFaulted)
                {
                    try
                    {
                        task.GetResult();
                    }
                    catch (Exception ex)
                    {
                        result.SetException(ex);
                    }
                }
                else if (task.IsCanceled)
                {
                    result.SetCanceled();
                }
                else
                {
                    result.SetResult();
                }
                RecyclablePool.Recycle(timeoutState);
            }, allowDuplicate: true);
            return result;
        }

        private static void WaitAsyncTimeoutCallback(object state)
        {
            WaitAsyncState timeoutState = (WaitAsyncState)state;
            lock (timeoutState)
            {
                if (timeoutState.Completed)
                {
                    return;
                }

                timeoutState.Completed = true;
                timeoutState.DelayTimer?.Dispose();
            }

            timeoutState.ResultTask.SetCanceled();
            RecyclablePool.Recycle(timeoutState);
        }

        /// <summary>
        /// WaitAsync状态管理
        /// </summary>
        private class WaitAsyncState : IRecycle
        {
            public StructTask ResultTask;
            public Timer DelayTimer;
            public bool Completed;

            public WaitAsyncState()
            {
            }

            public WaitAsyncState(StructTask resultTask)
            {
                ResultTask = resultTask;
            }

            public void Recycle()
            {
                DelayTimer?.Dispose();
                DelayTimer = null;
                ResultTask = default;
                Completed = false;
            }
        }

        /// <summary>
        /// WhenAny操作的状态
        /// </summary>
        private class WhenAnyState : IRecycle
        {
            public StructTask ResultTask;
            public int CompletedFlag;

            public void Recycle()
            {
                ResultTask = default;
                CompletedFlag = 0;
            }
        }

        /// <summary>
        /// WhenAny的延续操作
        /// </summary>
        private readonly struct WhenAnyContinuation
        {
            private readonly WhenAnyState _state;
            private readonly StructTask _task;

            public WhenAnyContinuation(WhenAnyState state, StructTask task)
            {
                _state = state;
                _task = task;
            }

            /// <summary>
            /// 执行延续操作
            /// </summary>
            /// <remarks>
            /// 使用Interlocked.Increment确保只有一个任务能设置结果。
            /// 只有第一个调用（返回1）的任务会设置结果。
            /// </remarks>
            public void Execute()
            {
                if (Interlocked.Increment(ref _state.CompletedFlag) == 1)
                {
                    if (_task.IsFaulted)
                    {
                        try
                        {
                            _task.GetResult();
                        }
                        catch (Exception ex)
                        {
                            _state.ResultTask.SetException(ex);
                        }
                    }
                    else if (_task.IsCanceled)
                    {
                        _state.ResultTask.SetCanceled();
                    }
                    else
                    {
                        _state.ResultTask.SetResult();
                    }
                    RecyclablePool.Recycle(_state);
                }
            }
        }

        /// <summary>
        /// WhenAll操作的状态
        /// </summary>
        private class WhenAllState : IRecycle
        {
            public StructTask ResultTask;
            public int Remaining;
            public List<Exception> Exceptions;
            public bool IsCanceled;

            public void Recycle()
            {
                ResultTask = default;
                Remaining = 0;
                Exceptions?.Clear();
                Exceptions = null;
                IsCanceled = false;
            }
        }

        /// <summary>
        /// WhenAll的延续操作
        /// </summary>
        private readonly struct WhenAllContinuation
        {
            private readonly WhenAllState _state;
            private readonly StructTask _task;

            public WhenAllContinuation(WhenAllState state, StructTask task)
            {
                _state = state;
                _task = task;
            }

            /// <summary>
            /// 执行延续操作
            /// </summary>
            /// <remarks>
            /// 收集所有异常，使用Interlocked.Decrement检测是否所有任务完成。
            /// 最后一个完成的任务负责设置最终结果。
            /// </remarks>
            public void Execute()
            {
                lock (_state)
                {
                    if (_task.IsFaulted)
                    {
                        _state.Exceptions = _state.Exceptions ?? new List<Exception>();
                        try
                        {
                            _task.GetResult();
                        }
                        catch (Exception ex)
                        {
                            _state.Exceptions.Add(ex);
                        }
                    }
                    else if (_task.IsCanceled)
                    {
                        _state.IsCanceled = true;
                    }
                }

                if (Interlocked.Decrement(ref _state.Remaining) == 0)
                {
                    if (_state.Exceptions != null)
                    {
                        _state.ResultTask.SetException(new AggregateException(_state.Exceptions));
                    }
                    else if (_state.IsCanceled)
                    {
                        _state.ResultTask.SetCanceled();
                    }
                    else
                    {
                        _state.ResultTask.SetResult();
                    }
                    RecyclablePool.Recycle(_state);
                }
            }
        }

        /// <summary>
        /// 任务回调队列管理器
        /// </summary>
        /// <remarks>
        /// <para>核心组件，负责管理所有任务的状态和回调执行。</para>
        /// <para>设计特点：</para>
        /// <list type="bullet">
        ///   <item><description>使用数组存储任务状态，O(1)访问时间</description></item>
        ///   <item><description>使用SpinLock实现细粒度锁，每个任务一个锁</description></item>
        ///   <item><description>使用版本号机制防止ABA问题</description></item>
        ///   <item><description>支持同步上下文感知的回调执行</description></item>
        /// </list>
        /// </remarks>
        private class CallbackQueue
        {
            /// <summary>
            /// 单例实例
            /// </summary>
            public static readonly CallbackQueue Instance = new CallbackQueue();

            /// <summary>
            /// 最大任务数量
            /// </summary>
            private readonly int _maxTasks;

            /// <summary>
            /// 空闲ID队列
            /// </summary>
            private readonly Queue<short> _freeIds;

            /// <summary>
            /// 下一个版本号
            /// </summary>
            private int _nextVersion;

            /// <summary>
            /// 空闲ID队列的锁
            /// </summary>
            private SpinLock _freeIdsLock;

            /// <summary>
            /// 每个任务的独立锁
            /// </summary>
            private readonly SpinLock[] _locks;

            /// <summary>
            /// 任务状态数组
            /// </summary>
            private readonly TaskState[] _states;

            private CallbackQueue()
            {
                _maxTasks = StructTaskOptions.MaxTasks;
                _freeIds = new Queue<short>(_maxTasks);
                _nextVersion = 1;
                _locks = new SpinLock[_maxTasks];
                for (short i = 1; i < _maxTasks; i++)
                {
                    _locks[i] = new SpinLock(false);
                }

                _states = new TaskState[_maxTasks];
                for (short i = 1; i < _maxTasks; i++)
                {
                    _freeIds.Enqueue(i);
                }
            }

            /// <summary>
            /// 分配新的任务ID和版本号
            /// </summary>
            /// <param name="manualRelease">是否手动释放，默认false（自动释放）</param>
            /// <returns>任务ID和版本号元组</returns>
            /// <exception cref="InvalidOperationException">超过最大任务数量</exception>
            public TaskData Allocate(bool manualRelease = false)
            {
                bool lockTaken = false;
                try
                {
                    _freeIdsLock.Enter(ref lockTaken);
                    if (_freeIds.Count == 0)
                    {
                        throw new InvalidOperationException($"Too many concurrent tasks (max: {_maxTasks})");
                    }

                    short id = _freeIds.Dequeue();
                    short version = (short)(Interlocked.Increment(ref _nextVersion) % short.MaxValue);
                    if (version == 0)
                    {
                        version = 1;
                    }

                    _states[id] = new TaskState(version, EStructTaskState.Pending, manualRelease: manualRelease);
                    return new TaskData(id, version);
                }
                finally
                {
                    if (lockTaken)
                    {
                        _freeIdsLock.Exit();
                    }
                }
            }

            /// <summary>
            /// 设置取消令牌注册
            /// </summary>
            /// <param name="id">任务ID</param>
            /// <param name="version">任务版本号</param>
            /// <param name="registration">取消令牌注册</param>
            public void SetCancellationRegistration(short id, short version, CancellationTokenRegistration registration)
            {
                bool lockTaken = false;
                try
                {
                    _locks[id].Enter(ref lockTaken);
                    TaskState state = _states[id];
                    if (state.Version == version && state.Status == EStructTaskState.Pending)
                    {
                        _states[id] = new TaskState(version, state.Status, state.Callback, state.Exception,
                            state.IsCallbackRegistered, registration, state.ManualRelease);
                    }
                }
                finally
                {
                    if (lockTaken)
                    {
                        _locks[id].Exit();
                    }
                }
            }

            /// <summary>
            /// 释放任务ID回空闲池
            /// </summary>
            /// <param name="id">要释放的ID</param>
            private void ReleaseId(short id)
            {
                bool lockTaken = false;
                try
                {
                    _freeIdsLock.Enter(ref lockTaken);
                    _freeIds.Enqueue(id);
                }
                finally
                {
                    if (lockTaken)
                    {
                        _freeIdsLock.Exit();
                    }
                }
            }

            /// <summary>
            /// 检查任务是否已完成
            /// </summary>
            public bool IsCompleted(short id, short version)
            {
                if (id <= 0 || id >= _maxTasks)
                {
                    return false;
                }

                TaskState state = _states[id];
                return state.Version == version && state.Status != EStructTaskState.Pending;
            }

            /// <summary>
            /// 检查任务是否成功完成
            /// </summary>
            public bool IsCompletedSuccessfully(short id, short version)
            {
                if (id <= 0 || id >= _maxTasks)
                {
                    return false;
                }

                TaskState state = _states[id];
                return state.Version == version && state.Status == EStructTaskState.Succeeded;
            }

            /// <summary>
            /// 检查任务是否失败
            /// </summary>
            public bool IsFaulted(short id, short version)
            {
                if (id <= 0 || id >= _maxTasks)
                {
                    return false;
                }

                TaskState state = _states[id];
                return state.Version == version && state.Status == EStructTaskState.Faulted;
            }

            /// <summary>
            /// 检查任务是否取消
            /// </summary>
            public bool IsCanceled(short id, short version)
            {
                if (id <= 0 || id >= _maxTasks)
                {
                    return false;
                }

                TaskState state = _states[id];
                return state.Version == version && state.Status == EStructTaskState.Canceled;
            }

            /// <summary>
            /// 注册完成回调
            /// </summary>
            /// <param name="id">任务ID</param>
            /// <param name="version">任务版本号</param>
            /// <param name="continuation">回调</param>
            /// <param name="allowDuplicate">是否允许重复注册</param>
            /// <param name="continueOnCapturedContext">是否捕获同步上下文</param>
            public void OnCompleted(short id, short version, Action continuation, bool allowDuplicate,
                bool continueOnCapturedContext)
            {
                bool lockTaken = false;
                try
                {
                    _locks[id].Enter(ref lockTaken);
                    TaskState state = _states[id];
                    if (state.Version != version)
                    {
                        throw new InvalidOperationException("Task version mismatch");
                    }

                    if (!allowDuplicate && state.IsCallbackRegistered)
                    {
                        throw new InvalidOperationException("Callback already registered");
                    }

                    if (state.Status != EStructTaskState.Pending)
                    {
                        ExecuteContinuation(continuation, continueOnCapturedContext);
                    }
                    else
                    {
                        _states[id] = new TaskState(version, state.Status, continuation, state.Exception,
                            true, state.CancellationRegistration, state.ManualRelease);
                    }
                }
                finally
                {
                    if (lockTaken)
                    {
                        _locks[id].Exit();
                    }
                }
            }

            /// <summary>
            /// 获取任务结果
            /// </summary>
            /// <param name="id">任务ID</param>
            /// <param name="version">任务版本号</param>
            public void GetResult(short id, short version)
            {
                bool lockTaken = false;
                try
                {
                    _locks[id].Enter(ref lockTaken);
                    TaskState state = _states[id];
                    if (state.Version != version)
                    {
                        throw new InvalidOperationException("Task version mismatch");
                    }

                    try
                    {
                        switch (state.Status)
                        {
                            case EStructTaskState.Faulted:
                                ExceptionDispatchInfo.Capture(state.Exception).Throw();
                                break;
                            case EStructTaskState.Canceled:
                                throw new OperationCanceledException("Task was canceled");
                            case EStructTaskState.Pending:
                                throw new InvalidOperationException("Task is still pending");
                        }
                    }
                    finally
                    {
                        if (!state.ManualRelease)
                        {
                            state.CancellationRegistration.Dispose();
                            _states[id] = default;
                            ReleaseId(id);
                        }
                    }
                }
                finally
                {
                    if (lockTaken)
                    {
                        _locks[id].Exit();
                    }
                }
            }

            /// <summary>
            /// 释放任务资源
            /// </summary>
            /// <param name="id">任务ID</param>
            /// <param name="version">任务版本号</param>
            public void Release(short id, short version)
            {
                bool lockTaken = false;
                try
                {
                    _locks[id].Enter(ref lockTaken);
                    TaskState state = _states[id];
                    if (state.Version != version)
                    {
                        return;
                    }

                    state.CancellationRegistration.Dispose();
                    _states[id] = default;
                    ReleaseId(id);
                }
                finally
                {
                    if (lockTaken)
                    {
                        _locks[id].Exit();
                    }
                }
            }

            /// <summary>
            /// 设置任务成功完成
            /// </summary>
            public void SetResult(short id, short version)
            {
                bool lockTaken = false;
                try
                {
                    _locks[id].Enter(ref lockTaken);
                    TaskState state = _states[id];
                    if (state.Version != version)
                    {
                        return;
                    }

                    if (state.Status != EStructTaskState.Pending)
                    {
                        throw new InvalidOperationException("Task already completed");
                    }

                    _states[id] = new TaskState(version, EStructTaskState.Succeeded, state.Callback, null,
                        state.IsCallbackRegistered, state.CancellationRegistration, state.ManualRelease);
                    ExecuteContinuation(state.Callback, true);
                }
                finally
                {
                    if (lockTaken)
                    {
                        _locks[id].Exit();
                    }
                }
            }

            /// <summary>
            /// 设置任务失败
            /// </summary>
            public void SetException(short id, short version, Exception exception)
            {
                bool lockTaken = false;
                try
                {
                    _locks[id].Enter(ref lockTaken);
                    TaskState state = _states[id];
                    if (state.Version != version)
                    {
                        return;
                    }

                    if (state.Status != EStructTaskState.Pending)
                    {
                        throw new InvalidOperationException("Task already completed");
                    }

                    _states[id] = new TaskState(version, EStructTaskState.Faulted, state.Callback, exception,
                        state.IsCallbackRegistered, state.CancellationRegistration, state.ManualRelease);
                    ExecuteContinuation(state.Callback, true);
                }
                finally
                {
                    if (lockTaken)
                    {
                        _locks[id].Exit();
                    }
                }
            }

            /// <summary>
            /// 设置任务取消
            /// </summary>
            public void SetCanceled(short id, short version)
            {
                bool lockTaken = false;
                try
                {
                    _locks[id].Enter(ref lockTaken);
                    TaskState state = _states[id];
                    if (state.Version != version)
                    {
                        return;
                    }

                    if (state.Status != EStructTaskState.Pending)
                    {
                        throw new InvalidOperationException("Task already completed");
                    }

                    _states[id] = new TaskState(version, EStructTaskState.Canceled, state.Callback, null,
                        state.IsCallbackRegistered, state.CancellationRegistration, state.ManualRelease);
                    ExecuteContinuation(state.Callback, true);
                }
                finally
                {
                    if (lockTaken)
                    {
                        _locks[id].Exit();
                    }
                }
            }

            /// <summary>
            /// 执行回调
            /// </summary>
            /// <param name="continuation">回调</param>
            /// <param name="continueOnCapturedContext">是否捕获同步上下文</param>
            /// <remarks>
            /// 根据配置决定回调执行方式：
            /// - 如果需要捕获上下文且有非默认同步上下文，使用SynchronizationContext.Post
            /// - 否则使用线程池执行
            /// </remarks>
            private void ExecuteContinuation(Action continuation, bool continueOnCapturedContext)
            {
                if (continuation == null)
                {
                    return;
                }

                if (continueOnCapturedContext)
                {
                    SynchronizationContext context = SynchronizationContext.Current;
                    if (context != null && context.GetType() != typeof(SynchronizationContext))
                    {
                        context.Post(_ => SafeInvoke(continuation), null);
                        return;
                    }
                }

                ThreadPool.UnsafeQueueUserWorkItem(_ => SafeInvoke(continuation), null);
            }

            /// <summary>
            /// 安全执行回调
            /// </summary>
            /// <param name="continuation">回调</param>
            /// <remarks>
            /// 捕获并报告回调中的异常，防止异常影响其他回调执行。
            /// </remarks>
            private void SafeInvoke(Action continuation)
            {
                try
                {
                    continuation.Invoke();
                }
                catch (Exception ex)
                {
                    StructTaskDiagnostics.ReportException(ex);
                }
            }
        }

        [StructLayout(LayoutKind.Auto)]
        private readonly struct TaskData
        {
            public readonly short Id;
            public readonly short Version;
            
            public TaskData(short id, short version)
            {
                Id = id;
                Version = version;
            }
        }
        
        /// <summary>
        /// 任务状态结构体
        /// </summary>
        /// <remarks>
        /// 使用StructLayout(LayoutKind.Auto)优化内存布局。
        /// 包含任务的所有状态信息。
        /// </remarks>
        [StructLayout(LayoutKind.Auto)]
        private readonly struct TaskState
        {
            public readonly Action Callback;
            public readonly Exception Exception;
            public readonly CancellationTokenRegistration CancellationRegistration;
            public readonly short Version;
            public readonly EStructTaskState Status;
            public readonly bool IsCallbackRegistered;
            public readonly bool ManualRelease;

            public TaskState(short version, EStructTaskState status, Action callback = null, Exception exception = null,
                bool isCallbackRegistered = false, CancellationTokenRegistration cancellationRegistration = default,
                bool manualRelease = false)
            {
                Callback = callback;
                Exception = exception;
                CancellationRegistration = cancellationRegistration;
                Version = version;
                Status = status;
                IsCallbackRegistered = isCallbackRegistered;
                ManualRelease = manualRelease;
            }
        }
    }

    /// <summary>
    /// 泛型轻量级可等待任务（支持返回值）
    /// </summary>
    /// <typeparam name="T">任务结果类型</typeparam>
    /// <remarks>
    /// <para>与StructTask类似，但支持返回值。</para>
    /// <para>所有方法的行为与非泛型版本相同，额外支持结果存储和获取。</para>
    /// </remarks>
    public readonly struct StructTask<T> : ICriticalNotifyCompletion
    {
        private static readonly CallbackQueue _callbackQueue = CallbackQueue.Instance;

        /// <summary>
        /// 已完成的默认任务（结果为default(T)）
        /// </summary>
        public static readonly StructTask<T> CompletedTask = CreateCompleted();

        private readonly short _id;
        private readonly short _version;
        private readonly StructTaskConfig _config;

        public bool IsCompleted => _callbackQueue.IsCompleted(_id, _version);
        public bool IsCompletedSuccessfully => _callbackQueue.IsCompletedSuccessfully(_id, _version);
        public bool IsFaulted => _callbackQueue.IsFaulted(_id, _version);
        public bool IsCanceled => _callbackQueue.IsCanceled(_id, _version);

        /// <summary>
        /// 获取任务是否有效
        /// </summary>
        /// <value>如果任务已初始化（ID不为0）则为true，否则为false</value>
        /// <remarks>
        /// ID=0表示未初始化的任务（default值）。
        /// 用于检查结构体是否已正确创建。
        /// </remarks>
        public bool IsValid => _id != 0;

        private StructTask(short id, short version, StructTaskConfig config = default)
        {
            _id = id;
            _version = version;
            _config = config;
        }

        /// <summary>
        /// 构造函数，创建新的StructTask&lt;T&gt;实例
        /// </summary>
        /// <param name="manualRelease">是否手动释放，默认false（自动释放）</param>
        /// <param name="cancellationToken">可选的取消令牌</param>
        /// <remarks>
        /// <para>便捷构造函数，内部调用Create方法。</para>
        /// <para>使用示例：</para>
        /// <code>
        /// StructTask&lt;int&gt; task = new StructTask&lt;int&gt;();
        /// // 等同于 StructTask&lt;int&gt; task = StructTask&lt;int&gt;.Create();
        /// </code>
        /// </remarks>
        public StructTask(bool manualRelease = false, CancellationToken cancellationToken = default)
        {
            this = Create(manualRelease, cancellationToken);
        }

        /// <summary>
        /// 创建已完成的任务单例
        /// </summary>
        /// <returns>已完成的StructTask&lt;T&gt;实例</returns>
        private static StructTask<T> CreateCompleted()
        {
            StructTask<T> task = Create();
            task.SetResult(default);
            return task;
        }

        /// <summary>
        /// 创建新的泛型任务
        /// </summary>
        /// <param name="manualRelease">是否手动释放，默认false（自动释放）</param>
        /// <param name="cancellationToken">可选的取消令牌</param>
        /// <returns>新创建的StructTask&lt;T&gt;实例</returns>
        /// <remarks>
        /// <para><b>释放模式：</b></para>
        /// <list type="bullet">
        ///   <item><description>自动释放（默认）：第一次await后自动释放资源</description></item>
        ///   <item><description>手动释放：需要调用Release()方法手动释放资源，支持多次await</description></item>
        /// </list>
        /// </remarks>
        public static StructTask<T> Create(bool manualRelease = false, CancellationToken cancellationToken = default)
        {
            TaskData taskData = _callbackQueue.Allocate(manualRelease);
            StructTask<T> task = new StructTask<T>(taskData.Id, taskData.Version);
            if (cancellationToken.CanBeCanceled)
            {
                CancellationTokenRegistration registration = cancellationToken.Register(() => task.SetCanceled());
                _callbackQueue.SetCancellationRegistration(taskData.Id, taskData.Version, registration);
            }

            return task;
        }

        /// <summary>
        /// 从结果创建已完成的任务
        /// </summary>
        /// <param name="result">任务结果</param>
        /// <returns>包含指定结果的已完成任务</returns>
        public static StructTask<T> FromResult(T result)
        {
            StructTask<T> task = Create();
            task.SetResult(result);
            return task;
        }

        /// <summary>
        /// 创建已失败的任务
        /// </summary>
        /// <param name="exception">导致失败的异常</param>
        /// <returns>已失败的StructTask&lt;T&gt;实例</returns>
        public static StructTask<T> FromException(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            StructTask<T> task = Create();
            task.SetException(exception);
            return task;
        }

        /// <summary>
        /// 创建已取消的任务
        /// </summary>
        /// <param name="cancellationToken">可选的取消令牌</param>
        /// <returns>已取消的StructTask&lt;T&gt;实例</returns>
        public static StructTask<T> FromCanceled(CancellationToken cancellationToken = default)
        {
            StructTask<T> task = Create();
            task.SetCanceled();
            return task;
        }

        /// <summary>
        /// 从Task&lt;T&gt;创建StructTask&lt;T&gt;
        /// </summary>
        /// <param name="task">源Task实例</param>
        /// <returns>对应的StructTask&lt;T&gt;实例</returns>
        public static StructTask<T> FromTask(Task<T> task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (task.IsCompleted)
            {
                if (task.IsFaulted)
                {
                    return FromException(task.Exception?.InnerException ?? new Exception("Task faulted"));
                }

                if (task.IsCanceled)
                {
                    return FromCanceled();
                }

                return FromResult(task.Result);
            }

            StructTask<T> result = Create();
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    result.SetException(t.Exception?.InnerException ?? new Exception("Task faulted"));
                }
                else if (t.IsCanceled)
                {
                    result.SetCanceled();
                }
                else
                {
                    result.SetResult(t.Result);
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
            return result;
        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        /// <summary>
        /// 从ValueTask&lt;T&gt;创建StructTask&lt;T&gt;
        /// </summary>
        /// <param name="valueTask">源ValueTask实例</param>
        /// <returns>对应的StructTask&lt;T&gt;实例</returns>
        /// <remarks>
        /// <para>将ValueTask转换为StructTask，实现与ValueTask的互操作。</para>
        /// <para>如果ValueTask已完成，直接返回对应状态；否则将其转换为Task后处理。</para>
        /// </remarks>
        public static StructTask<T> FromValueTask(ValueTask<T> valueTask)
        {
            if (valueTask.IsCompleted)
            {
                if (valueTask.IsFaulted)
                {
                    try { valueTask.GetAwaiter().GetResult(); }
                    catch (Exception ex) { return FromException(ex); }
                }
                if (valueTask.IsCanceled)
                {
                    return FromCanceled();
                }
                return FromResult(valueTask.Result);
            }
            StructTask<T> result = Create();
            valueTask.AsTask().ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    result.SetException(t.Exception?.InnerException ?? new Exception("Task faulted"));
                }
                else if (t.IsCanceled)
                {
                    result.SetCanceled();
                }
                else
                {
                    result.SetResult(t.Result);
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
            return result;
        }
#endif

        /// <summary>
        /// 将StructTask&lt;T&gt;转换为Task&lt;T&gt;
        /// </summary>
        /// <returns>对应的Task&lt;T&gt;实例</returns>
        /// <remarks>
        /// <para>实现与Task的互操作。</para>
        /// <para>转换逻辑：</para>
        /// <list type="bullet">
        ///   <item><description>已完成任务：直接返回对应状态的Task</description></item>
        ///   <item><description>未完成任务：创建TaskCompletionSource，任务完成时设置结果</description></item>
        /// </list>
        /// </remarks>
        public Task<T> AsTask()
        {
            StructTask<T> self = this;
            if (self.IsCompleted)
            {
                if (self.IsFaulted)
                {
                    try
                    {
                        self.GetResult();
                    }
                    catch (Exception ex)
                    {
                        return Task.FromException<T>(ex);
                    }
                }

                if (self.IsCanceled)
                {
                    return Task.FromCanceled<T>(new CancellationToken(self.IsCanceled));
                }

                return Task.FromResult(self.GetResult());
            }

            TaskCompletionSource<T> tcs2 = new TaskCompletionSource<T>();
            self.OnCompleted(() =>
            {
                if (self.IsFaulted)
                {
                    try
                    {
                        self.GetResult();
                    }
                    catch (Exception ex)
                    {
                        tcs2.SetException(ex);
                    }
                }
                else if (self.IsCanceled)
                {
                    tcs2.SetCanceled();
                }
                else
                {
                    tcs2.SetResult(self.GetResult());
                }
            }, allowDuplicate: true);
            return tcs2.Task;
        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        /// <summary>
        /// 将StructTask&lt;T&gt;转换为ValueTask&lt;T&gt;
        /// </summary>
        /// <returns>对应的ValueTask&lt;T&gt;实例</returns>
        /// <remarks>
        /// 仅适用于.NET Standard 2.1及以上版本。
        /// 通过AsTask()转换后包装为ValueTask。
        /// </remarks>
        public ValueTask<T> AsValueTask()
        {
            return new ValueTask<T>(AsTask());
        }
#endif

        /// <summary>
        /// 配置await行为
        /// </summary>
        /// <param name="continueOnCapturedContext">
        /// true：捕获当前同步上下文，回调在捕获的上下文执行；
        /// false：不捕获上下文，回调在线程池执行
        /// </param>
        /// <returns>配置后的等待器</returns>
        public ConfiguredStructTask<T> ConfigureAwait(bool continueOnCapturedContext)
        {
            return new ConfiguredStructTask<T>(this, continueOnCapturedContext);
        }

        /// <summary>
        /// 获取等待器（支持await语法）
        /// </summary>
        /// <returns>当前StructTask&lt;T&gt;实例</returns>
        /// <remarks>
        /// 实现await模式必需的方法。
        /// 编译器会将 await task 转换为 task.GetAwaiter()。
        /// </remarks>
        public StructTask<T> GetAwaiter()
        {
            return this;
        }

        /// <summary>
        /// 获取任务结果（阻塞等待）
        /// </summary>
        /// <returns>任务结果</returns>
        /// <remarks>
        /// <para>实现await模式必需的方法。</para>
        /// <para><b>释放行为：</b></para>
        /// <list type="bullet">
        ///   <item><description>自动释放模式（默认）：第一次调用后自动释放资源</description></item>
        ///   <item><description>手动释放模式：不会自动释放，需要调用Release()手动释放</description></item>
        /// </list>
        /// </remarks>
        /// <exception cref="InvalidOperationException">任务未初始化或仍在待定状态</exception>
        /// <exception cref="Exception">任务失败时重新抛出原始异常</exception>
        /// <exception cref="OperationCanceledException">任务被取消时抛出</exception>
        public T GetResult()
        {
            if (_id == 0)
            {
                throw new InvalidOperationException("StructTask is not initialized");
            }

            return _callbackQueue.GetResult(_id, _version);
        }

        /// <summary>
        /// 注册完成回调
        /// </summary>
        /// <param name="continuation">任务完成时执行的回调</param>
        /// <remarks>
        /// <para>实现await模式必需的方法。</para>
        /// <para>如果任务已完成，回调会被立即执行；否则回调会被存储，任务完成时执行。</para>
        /// <para><b>注意：</b>默认情况下不允许重复注册回调，会抛出异常。</para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">任务未初始化或回调已注册</exception>
        public void OnCompleted(Action continuation)
        {
            if (_id == 0)
            {
                throw new InvalidOperationException("StructTask is not initialized");
            }

            _callbackQueue.OnCompleted(_id, _version, continuation, allowDuplicate: false,
                _config.ContinueOnCapturedContext);
        }

        /// <summary>
        /// 注册完成回调（内部方法，允许重复注册）
        /// </summary>
        /// <param name="continuation">任务完成时执行的回调</param>
        /// <param name="allowDuplicate">是否允许重复注册回调</param>
        /// <param name="continueOnCapturedContext">是否捕获同步上下文</param>
        /// <remarks>
        /// 内部使用的方法，支持更灵活的回调注册选项。
        /// WhenAny/WhenAll等组合操作使用此方法。
        /// </remarks>
        internal void OnCompleted(Action continuation, bool allowDuplicate, bool continueOnCapturedContext = true)
        {
            if (_id == 0)
            {
                throw new InvalidOperationException("StructTask is not initialized");
            }

            _callbackQueue.OnCompleted(_id, _version, continuation, allowDuplicate, continueOnCapturedContext);
        }

        /// <summary>
        /// 不安全注册完成回调
        /// </summary>
        /// <param name="continuation">任务完成时执行的回调</param>
        /// <remarks>
        /// <para>实现ICriticalNotifyCompletion接口的方法。</para>
        /// <para>与OnCompleted的区别：不捕获执行上下文，性能更高但安全性更低。</para>
        /// </remarks>
        public void UnsafeOnCompleted(Action continuation)
        {
            OnCompleted(continuation);
        }

        /// <summary>
        /// 设置任务成功完成
        /// </summary>
        /// <param name="result">任务结果</param>
        /// <remarks>
        /// <para>将任务状态从Pending转换为Succeeded。</para>
        /// <para>调用此方法后，所有等待的回调会被调度执行。</para>
        /// <para><b>注意：</b>每个任务只能调用一次SetResult/SetException/SetCanceled。</para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">任务未初始化或已完成</exception>
        public void SetResult(T result)
        {
            if (_id == 0)
            {
                throw new InvalidOperationException("StructTask is not initialized");
            }

            _callbackQueue.SetResult(_id, _version, result);
        }

        /// <summary>
        /// 设置任务失败
        /// </summary>
        /// <param name="exception">导致失败的异常</param>
        /// <remarks>
        /// <para>将任务状态从Pending转换为Faulted。</para>
        /// <para>调用此方法后，所有等待的回调会被调度执行。</para>
        /// <para>await此任务时，异常会被重新抛出，保留原始堆栈跟踪。</para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">任务未初始化或已完成</exception>
        /// <exception cref="ArgumentNullException">exception为null</exception>
        public void SetException(Exception exception)
        {
            if (_id == 0)
            {
                throw new InvalidOperationException("StructTask is not initialized");
            }

            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            _callbackQueue.SetException(_id, _version, exception);
        }

        /// <summary>
        /// 设置任务取消
        /// </summary>
        /// <remarks>
        /// <para>将任务状态从Pending转换为Canceled。</para>
        /// <para>调用此方法后，所有等待的回调会被调度执行。</para>
        /// <para>await此任务时，会抛出OperationCanceledException。</para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">任务未初始化或已完成</exception>
        public void SetCanceled()
        {
            if (_id == 0)
            {
                throw new InvalidOperationException("StructTask is not initialized");
            }

            _callbackQueue.SetCanceled(_id, _version);
        }

        /// <summary>
        /// 释放任务资源
        /// </summary>
        /// <remarks>
        /// <para>释放任务占用的资源，将ID归还到空闲池。</para>
        /// <para><b>注意：</b>调用此方法后，任务将无法再被await或访问。</para>
        /// <para>如果任务需要被多次await，可以在最后一次使用完毕后调用此方法。</para>
        /// </remarks>
        public void Release()
        {
            if (_id == 0)
            {
                return;
            }

            _callbackQueue.Release(_id, _version);
        }

        /// <summary>
        /// 任务完成后执行延续操作
        /// </summary>
        /// <param name="continuation">延续操作，接收完成的任务作为参数</param>
        /// <returns>代表延续操作的新任务</returns>
        /// <remarks>
        /// <para>类似于Task.ContinueWith的功能。</para>
        /// <para>延续操作会在原任务完成后执行，无论成功、失败还是取消。</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">continuation为null</exception>
        public StructTask ContinueWith(Action<StructTask<T>> continuation)
        {
            if (continuation == null)
            {
                throw new ArgumentNullException(nameof(continuation));
            }

            StructTask<T> self = this;
            StructTask result = StructTask.Create();
            OnCompleted(() =>
            {
                try
                {
                    continuation(self);
                    result.SetResult();
                }
                catch (Exception ex)
                {
                    result.SetException(ex);
                }
            }, allowDuplicate: true);
            return result;
        }

        /// <summary>
        /// 任务完成后执行延续操作并返回结果
        /// </summary>
        /// <typeparam name="TResult">延续操作的返回类型</typeparam>
        /// <param name="continuation">延续操作，接收完成的任务并返回结果</param>
        /// <returns>代表延续操作结果的新任务</returns>
        /// <remarks>
        /// <para>类似于Task&lt;TResult&gt;.ContinueWith的功能。</para>
        /// <para>延续操作的返回值会成为新任务的结果。</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">continuation为null</exception>
        public StructTask<TResult> ContinueWith<TResult>(Func<StructTask<T>, TResult> continuation)
        {
            if (continuation == null)
            {
                throw new ArgumentNullException(nameof(continuation));
            }

            StructTask<T> self = this;
            StructTask<TResult> result = StructTask<TResult>.Create();
            OnCompleted(() =>
            {
                try
                {
                    TResult value = continuation(self);
                    result.SetResult(value);
                }
                catch (Exception ex)
                {
                    result.SetException(ex);
                }
            }, allowDuplicate: true);
            return result;
        }

        /// <summary>
        /// 等待任意一个任务完成
        /// </summary>
        /// <param name="tasks">要等待的任务数组</param>
        /// <returns>第一个完成的任务</returns>
        /// <remarks>
        /// <para>类似于Task.WhenAny的功能。</para>
        /// <para>快速路径：如果已有任务完成，立即返回该任务。</para>
        /// <para>慢速路径：注册所有任务的回调，第一个完成的任务触发结果。</para>
        /// </remarks>
        /// <exception cref="ArgumentException">tasks为null或空数组</exception>
        public static StructTask<T> WhenAny(params StructTask<T>[] tasks)
        {
            if (tasks == null || tasks.Length == 0)
            {
                throw new ArgumentException("Tasks cannot be null or empty", nameof(tasks));
            }

            for (int i = 0; i < tasks.Length; i++)
            {
                if (tasks[i].IsCompleted)
                {
                    return tasks[i];
                }
            }

            StructTask<T> result = Create();
            WhenAnyState<T> state = RecyclablePool.Acquire<WhenAnyState<T>>();
            state.ResultTask = result;
            for (int i = 0; i < tasks.Length; i++)
            {
                StructTask<T> task = tasks[i];
                WhenAnyContinuation<T> continuation = new WhenAnyContinuation<T>(state, task);
                task.OnCompleted(continuation.Execute, allowDuplicate: true);
            }

            return result;
        }

        /// <summary>
        /// 等待所有任务完成
        /// </summary>
        /// <param name="tasks">要等待的任务数组</param>
        /// <returns>包含所有任务结果的数组</returns>
        /// <remarks>
        /// <para>类似于Task.WhenAll的功能。</para>
        /// <para>快速路径：如果所有任务已完成，立即返回结果数组。</para>
        /// <para>异常处理：</para>
        /// <list type="bullet">
        ///   <item><description>如果有任务失败，返回AggregateException包含所有异常</description></item>
        ///   <item><description>如果有任务取消（无异常），返回取消状态</description></item>
        ///   <item><description>如果全部成功，返回成功及结果数组</description></item>
        /// </list>
        /// </remarks>
        /// <exception cref="ArgumentException">tasks为null</exception>
        public static StructTask<T[]> WhenAll(params StructTask<T>[] tasks)
        {
            if (tasks == null || tasks.Length == 0)
            {
                return StructTask<T[]>.FromResult(Array.Empty<T>());
            }

            int pendingCount = 0;
            List<Exception> exceptions = null;
            bool anyCanceled = false;
            for (int i = 0; i < tasks.Length; i++)
            {
                if (!tasks[i].IsCompleted)
                {
                    pendingCount++;
                }
                else if (tasks[i].IsFaulted)
                {
                    exceptions = exceptions ?? new List<Exception>();
                    try
                    {
                        tasks[i].GetResult();
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }
                else if (tasks[i].IsCanceled)
                {
                    anyCanceled = true;
                }
            }

            if (pendingCount == 0)
            {
                if (exceptions != null)
                {
                    return StructTask<T[]>.FromException(new AggregateException(exceptions));
                }

                if (anyCanceled)
                {
                    return StructTask<T[]>.FromCanceled();
                }

                T[] results = new T[tasks.Length];
                for (int i = 0; i < tasks.Length; i++)
                {
                    results[i] = tasks[i].GetResult();
                }

                return StructTask<T[]>.FromResult(results);
            }

            StructTask<T[]> result = StructTask<T[]>.Create();
            WhenAllState<T> state = RecyclablePool.Acquire<WhenAllState<T>>();
            state.Results = new T[tasks.Length];
            state.Remaining = pendingCount;
            state.ResultTask = result;
            state.Exceptions = exceptions;
            state.IsCanceled = anyCanceled;
            for (int i = 0; i < tasks.Length; i++)
            {
                if (!tasks[i].IsCompleted)
                {
                    StructTask<T> task = tasks[i];
                    int index = i;
                    WhenAllContinuation<T> continuation = new WhenAllContinuation<T>(state, task, index);
                    task.OnCompleted(continuation.Execute, allowDuplicate: true);
                }
                else if (tasks[i].IsCompletedSuccessfully)
                {
                    state.Results[i] = tasks[i].GetResult();
                }
            }

            return result;
        }

        private class WhenAnyState<TState> : IRecycle
        {
            public StructTask<TState> ResultTask;
            public int CompletedFlag;

            public void Recycle()
            {
                ResultTask = default;
                CompletedFlag = 0;
            }
        }

        private readonly struct WhenAnyContinuation<TState>
        {
            private readonly WhenAnyState<TState> _state;
            private readonly StructTask<TState> _task;

            public WhenAnyContinuation(WhenAnyState<TState> state, StructTask<TState> task)
            {
                _state = state;
                _task = task;
            }

            public void Execute()
            {
                if (Interlocked.Increment(ref _state.CompletedFlag) == 1)
                {
                    if (_task.IsFaulted)
                    {
                        try
                        {
                            _task.GetResult();
                        }
                        catch (Exception ex)
                        {
                            _state.ResultTask.SetException(ex);
                        }
                    }
                    else if (_task.IsCanceled)
                    {
                        _state.ResultTask.SetCanceled();
                    }
                    else
                    {
                        _state.ResultTask.SetResult(_task.GetResult());
                    }
                    RecyclablePool.Recycle(_state);
                }
            }
        }

        private class WhenAllState<TState> : IRecycle
        {
            public StructTask<TState[]> ResultTask;
            public TState[] Results;
            public int Remaining;
            public List<Exception> Exceptions;
            public bool IsCanceled;

            public WhenAllState()
            {
            }

            public WhenAllState(int count, StructTask<TState[]> resultTask)
            {
                Results = new TState[count];
                Remaining = count;
                ResultTask = resultTask;
            }

            public void Recycle()
            {
                ResultTask = default;
                Results = null;
                Remaining = 0;
                Exceptions?.Clear();
                Exceptions = null;
                IsCanceled = false;
            }
        }

        private readonly struct WhenAllContinuation<TState>
        {
            private readonly WhenAllState<TState> _state;
            private readonly StructTask<TState> _task;
            private readonly int _index;

            public WhenAllContinuation(WhenAllState<TState> state, StructTask<TState> task, int index)
            {
                _state = state;
                _task = task;
                _index = index;
            }

            public void Execute()
            {
                lock (_state)
                {
                    if (_task.IsFaulted)
                    {
                        _state.Exceptions = _state.Exceptions ?? new List<Exception>();
                        try
                        {
                            _task.GetResult();
                        }
                        catch (Exception ex)
                        {
                            _state.Exceptions.Add(ex);
                        }
                    }
                    else if (_task.IsCanceled)
                    {
                        _state.IsCanceled = true;
                    }
                    else
                    {
                        _state.Results[_index] = _task.GetResult();
                    }
                }

                if (Interlocked.Decrement(ref _state.Remaining) == 0)
                {
                    if (_state.Exceptions != null)
                    {
                        _state.ResultTask.SetException(new AggregateException(_state.Exceptions));
                    }
                    else if (_state.IsCanceled)
                    {
                        _state.ResultTask.SetCanceled();
                    }
                    else
                    {
                        _state.ResultTask.SetResult(_state.Results);
                    }
                    RecyclablePool.Recycle(_state);
                }
            }
        }

        private class CallbackQueue
        {
            public static readonly CallbackQueue Instance = new CallbackQueue();
            private readonly int _maxTasks;
            private readonly Queue<short> _freeIds;
            private int _nextVersion;
            private SpinLock _freeIdsLock;
            private readonly SpinLock[] _locks;
            private readonly TaskState[] _states;

            private CallbackQueue()
            {
                _maxTasks = StructTaskOptions.MaxTasks;
                _freeIds = new Queue<short>(_maxTasks);
                _nextVersion = 1;
                _locks = new SpinLock[_maxTasks];
                for (short i = 1; i < _maxTasks; i++)
                {
                    _locks[i] = new SpinLock(false);
                }

                _states = new TaskState[_maxTasks];
                for (short i = 1; i < _maxTasks; i++)
                {
                    _freeIds.Enqueue(i);
                }
            }

            public TaskData Allocate(bool manualRelease = false)
            {
                bool lockTaken = false;
                try
                {
                    _freeIdsLock.Enter(ref lockTaken);
                    if (_freeIds.Count == 0)
                    {
                        throw new InvalidOperationException($"Too many concurrent tasks (max: {_maxTasks})");
                    }

                    short id = _freeIds.Dequeue();
                    short version = (short)(Interlocked.Increment(ref _nextVersion) % short.MaxValue);
                    if (version == 0)
                    {
                        version = 1;
                    }

                    _states[id] = new TaskState(version, EStructTaskState.Pending, manualRelease: manualRelease);
                    return new TaskData(id, version);
                }
                finally
                {
                    if (lockTaken)
                    {
                        _freeIdsLock.Exit();
                    }
                }
            }

            public void SetCancellationRegistration(short id, short version, CancellationTokenRegistration registration)
            {
                bool lockTaken = false;
                try
                {
                    _locks[id].Enter(ref lockTaken);
                    TaskState state = _states[id];
                    if (state.Version == version && state.Status == EStructTaskState.Pending)
                    {
                        _states[id] = new TaskState(version, state.Status, state.Callback, state.Result,
                            state.Exception,
                            state.IsCallbackRegistered, registration, state.ManualRelease);
                    }
                }
                finally
                {
                    if (lockTaken)
                    {
                        _locks[id].Exit();
                    }
                }
            }

            private void ReleaseId(short id)
            {
                bool lockTaken = false;
                try
                {
                    _freeIdsLock.Enter(ref lockTaken);
                    _freeIds.Enqueue(id);
                }
                finally
                {
                    if (lockTaken)
                    {
                        _freeIdsLock.Exit();
                    }
                }
            }

            public bool IsCompleted(short id, short version)
            {
                if (id <= 0 || id >= _maxTasks)
                {
                    return false;
                }

                TaskState state = _states[id];
                return state.Version == version && state.Status != EStructTaskState.Pending;
            }

            public bool IsCompletedSuccessfully(short id, short version)
            {
                if (id <= 0 || id >= _maxTasks)
                {
                    return false;
                }

                TaskState state = _states[id];
                return state.Version == version && state.Status == EStructTaskState.Succeeded;
            }

            public bool IsFaulted(short id, short version)
            {
                if (id <= 0 || id >= _maxTasks)
                {
                    return false;
                }

                TaskState state = _states[id];
                return state.Version == version && state.Status == EStructTaskState.Faulted;
            }

            public bool IsCanceled(short id, short version)
            {
                if (id <= 0 || id >= _maxTasks)
                {
                    return false;
                }

                TaskState state = _states[id];
                return state.Version == version && state.Status == EStructTaskState.Canceled;
            }

            public void OnCompleted(short id, short version, Action continuation, bool allowDuplicate,
                bool continueOnCapturedContext)
            {
                bool lockTaken = false;
                try
                {
                    _locks[id].Enter(ref lockTaken);
                    TaskState state = _states[id];
                    if (state.Version != version)
                    {
                        throw new InvalidOperationException("Task version mismatch");
                    }

                    if (!allowDuplicate && state.IsCallbackRegistered)
                    {
                        throw new InvalidOperationException("Callback already registered");
                    }

                    if (state.Status != EStructTaskState.Pending)
                    {
                        ExecuteContinuation(continuation, continueOnCapturedContext);
                    }
                    else
                    {
                        _states[id] = new TaskState(version, state.Status, continuation, state.Result, state.Exception,
                            true, state.CancellationRegistration, state.ManualRelease);
                    }
                }
                finally
                {
                    if (lockTaken)
                    {
                        _locks[id].Exit();
                    }
                }
            }

            public T GetResult(short id, short version)
            {
                bool lockTaken = false;
                try
                {
                    _locks[id].Enter(ref lockTaken);
                    TaskState state = _states[id];
                    if (state.Version != version)
                    {
                        throw new InvalidOperationException("Task version mismatch");
                    }

                    try
                    {
                        switch (state.Status)
                        {
                            case EStructTaskState.Faulted:
                                ExceptionDispatchInfo.Capture(state.Exception).Throw();
                                return default;
                            case EStructTaskState.Canceled:
                                throw new OperationCanceledException("Task was canceled");
                            case EStructTaskState.Pending:
                                throw new InvalidOperationException("Task is still pending");
                            default:
                                return state.Result;
                        }
                    }
                    finally
                    {
                        if (!state.ManualRelease)
                        {
                            state.CancellationRegistration.Dispose();
                            _states[id] = default;
                            ReleaseId(id);
                        }
                    }
                }
                finally
                {
                    if (lockTaken)
                    {
                        _locks[id].Exit();
                    }
                }
            }

            /// <summary>
            /// 释放任务资源
            /// </summary>
            /// <param name="id">任务ID</param>
            /// <param name="version">任务版本号</param>
            public void Release(short id, short version)
            {
                bool lockTaken = false;
                try
                {
                    _locks[id].Enter(ref lockTaken);
                    TaskState state = _states[id];
                    if (state.Version != version)
                    {
                        return;
                    }

                    state.CancellationRegistration.Dispose();
                    _states[id] = default;
                    ReleaseId(id);
                }
                finally
                {
                    if (lockTaken)
                    {
                        _locks[id].Exit();
                    }
                }
            }

            public void SetResult(short id, short version, T result)
            {
                bool lockTaken = false;
                try
                {
                    _locks[id].Enter(ref lockTaken);
                    TaskState state = _states[id];
                    if (state.Version != version)
                    {
                        return;
                    }

                    if (state.Status != EStructTaskState.Pending)
                    {
                        throw new InvalidOperationException("Task already completed");
                    }

                    _states[id] = new TaskState(version, EStructTaskState.Succeeded, state.Callback, result, null,
                        state.IsCallbackRegistered, state.CancellationRegistration, state.ManualRelease);
                    ExecuteContinuation(state.Callback, true);
                }
                finally
                {
                    if (lockTaken)
                    {
                        _locks[id].Exit();
                    }
                }
            }

            public void SetException(short id, short version, Exception exception)
            {
                bool lockTaken = false;
                try
                {
                    _locks[id].Enter(ref lockTaken);
                    TaskState state = _states[id];
                    if (state.Version != version)
                    {
                        return;
                    }

                    if (state.Status != EStructTaskState.Pending)
                    {
                        throw new InvalidOperationException("Task already completed");
                    }

                    _states[id] = new TaskState(version, EStructTaskState.Faulted, state.Callback, state.Result,
                        exception,
                        state.IsCallbackRegistered, state.CancellationRegistration, state.ManualRelease);
                    ExecuteContinuation(state.Callback, true);
                }
                finally
                {
                    if (lockTaken)
                    {
                        _locks[id].Exit();
                    }
                }
            }

            public void SetCanceled(short id, short version)
            {
                bool lockTaken = false;
                try
                {
                    _locks[id].Enter(ref lockTaken);
                    TaskState state = _states[id];
                    if (state.Version != version)
                    {
                        return;
                    }

                    if (state.Status != EStructTaskState.Pending)
                    {
                        throw new InvalidOperationException("Task already completed");
                    }

                    _states[id] = new TaskState(version, EStructTaskState.Canceled, state.Callback, state.Result, null,
                        state.IsCallbackRegistered, state.CancellationRegistration, state.ManualRelease);
                    ExecuteContinuation(state.Callback, true);
                }
                finally
                {
                    if (lockTaken)
                    {
                        _locks[id].Exit();
                    }
                }
            }

            private void ExecuteContinuation(Action continuation, bool continueOnCapturedContext)
            {
                if (continuation == null)
                {
                    return;
                }

                if (continueOnCapturedContext)
                {
                    SynchronizationContext context = SynchronizationContext.Current;
                    if (context != null && context.GetType() != typeof(SynchronizationContext))
                    {
                        context.Post(_ => SafeInvoke(continuation), null);
                        return;
                    }
                }

                ThreadPool.UnsafeQueueUserWorkItem(_ => SafeInvoke(continuation), null);
            }

            private void SafeInvoke(Action continuation)
            {
                try
                {
                    continuation.Invoke();
                }
                catch (Exception ex)
                {
                    StructTaskDiagnostics.ReportException(ex);
                }
            }
        }

        [StructLayout(LayoutKind.Auto)]
        private readonly struct TaskData
        {
            public readonly short Id;
            public readonly short Version;
            
            public TaskData(short id, short version)
            {
                Id = id;
                Version = version;
            }
        }

        [StructLayout(LayoutKind.Auto)]
        private readonly struct TaskState
        {
            public readonly Action Callback;
            public readonly T Result;
            public readonly Exception Exception;
            public readonly CancellationTokenRegistration CancellationRegistration;
            public readonly short Version;
            public readonly EStructTaskState Status;
            public readonly bool IsCallbackRegistered;
            public readonly bool ManualRelease;

            public TaskState(short version, EStructTaskState status, Action callback = null, T result = default,
                Exception exception = null, bool isCallbackRegistered = false,
                CancellationTokenRegistration cancellationRegistration = default,
                bool manualRelease = false)
            {
                Callback = callback;
                Result = result;
                Exception = exception;
                CancellationRegistration = cancellationRegistration;
                Version = version;
                Status = status;
                IsCallbackRegistered = isCallbackRegistered;
                ManualRelease = manualRelease;
            }
        }
    }

    /// <summary>
    /// 任务配置
    /// </summary>
    public readonly struct StructTaskConfig
    {
        /// <summary>
        /// 是否捕获同步上下文
        /// </summary>
        public readonly bool ContinueOnCapturedContext;

        public StructTaskConfig(bool continueOnCapturedContext)
        {
            ContinueOnCapturedContext = continueOnCapturedContext;
        }
    }
}