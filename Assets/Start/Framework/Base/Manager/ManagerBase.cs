using System.Threading.Tasks;

namespace Start.Framework
{
    public abstract class ManagerBase<T> : IManager where T : ManagerBase<T>
    {
        public static T Instance => _instance ?? (_instance = Manager.GetManger<T>());

        public abstract int Priority { get; }

        private static T _instance;

        public virtual Task Initialize()
        {
            return Task.CompletedTask;
        }

        public virtual Task DeInitialize()
        {
            _instance = null;
            return Task.CompletedTask;
        }
    }
}