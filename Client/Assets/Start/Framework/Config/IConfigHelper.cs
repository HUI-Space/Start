using System;
using System.Threading.Tasks;

namespace Start
{
    public interface IConfigHelper
    {
        IConfig LoadConfig(Type type);
    }
}