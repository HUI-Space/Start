﻿namespace Start.Editor.ResourceEditor
{
    public interface IResourceBuilder
    {
        T GetData<T>(string key);
        void SetData<T>(string key, T data);
    }
}