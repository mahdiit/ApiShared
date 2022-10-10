using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Core.CacheHandler
{
    public class MemoryCacheProvider : ICacheProvider
    {
        protected readonly IMemoryCache CacheProvider;

        public MemoryCacheProvider(IMemoryCache memoryCache)
        {
            CacheProvider = memoryCache;
        }

        public virtual void Clear()
        {

        }

        public virtual T? GetData<T>(string cacheKey, Func<T> fallbackFunction, TimeSpan expireTime) where T : class
        {
            T data;
            if (CacheProvider.TryGetValue(cacheKey, out data))
            {
                return data;
            }

            data = fallbackFunction();
            if (data != null)
            {
                var op = new MemoryCacheEntryOptions() { AbsoluteExpiration = DateTime.Now + expireTime };
                CacheProvider.Set(cacheKey, data, op);
            }
            return data;
        }

        public virtual void Remove(string cacheKey)
        {
            CacheProvider.Remove(cacheKey);
        }

        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Clear();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
