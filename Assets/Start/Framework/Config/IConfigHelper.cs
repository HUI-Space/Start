using System;

namespace Start.Framework
{
    public interface IConfigHelper
    {
        IConfig LoadConfig(Type type);
    }
}