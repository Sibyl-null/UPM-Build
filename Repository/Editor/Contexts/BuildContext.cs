using System;
using System.Collections.Generic;

namespace Build.Editor.Contexts
{
    public class BuildContext
    {
        private readonly Dictionary<string, object> _contextDataDic = new();

        public T Get<T>(string key)
        {
            if (_contextDataDic.ContainsKey(key) == false)
                throw new Exception($"BuildContext 不存在 {key} 参数");

            T data = (T)_contextDataDic[key];
            return data;
        }

        public void Set<T>(string key, T data)
        {
            _contextDataDic[key] = data;
        }
    }
}