﻿using Start.Framework;

namespace Start.Runtime
{
    public class LogHelper:ILogHelper
    {
        public void Log(ELogType logType, string message, params object[] args)
        {
            switch (logType)
            {
                case ELogType.Info:
                    UnityEngine.Debug.Log($"Info:{message}");
                    break;
                case ELogType.Warning:
                    UnityEngine.Debug.LogWarning($"Warning:{message}");
                    break;
                case ELogType.Error:
                    UnityEngine.Debug.LogError($"Error:{message}");
                    break;
                case ELogType.Fatal:
                    UnityEngine.Debug.LogError($"Fatal:{message}");
                    break;
            }
        }
    }
}