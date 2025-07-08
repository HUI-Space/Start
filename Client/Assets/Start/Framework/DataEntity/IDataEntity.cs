using System;

namespace Start
{
    public interface IDataEntity
    {
        void Initialize();
        void ResetTempData();
        void TryRefresh();
        void Reset();
        void DeInitialize();
    }
}