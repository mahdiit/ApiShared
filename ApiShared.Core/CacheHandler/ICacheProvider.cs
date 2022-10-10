using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Core.CacheHandler
{
    public interface ICacheProvider : IDisposable
    {
        void Clear();
        T? GetData<T>(string cacheKey, Func<T> fallbackFunction, TimeSpan expireTime) where T : class;
        void Remove(string key);
    }
}
