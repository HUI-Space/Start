using System;

namespace Start.Framework
{
    public interface IDataEntity
    {
        void Initialize();
        void RegisterNetUpdate<TNet>(TNet net, Action<TNet> callback) where TNet : DataSetBase;
        void ResetTempData();
        void TryRefresh();
        void Reset();
        void DeInitialize();
    }
}