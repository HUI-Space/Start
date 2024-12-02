using System.Threading.Tasks;

namespace Start.Framework
{
    public interface IManager
    {
        int Priority { get; }

        Task Initialize();

        Task DeInitialize();
    }
}