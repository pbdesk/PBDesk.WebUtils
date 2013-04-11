using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PBDesk.WebUtils.CacheProviders
{
    public interface ICacheProvider
    {
        object Get(string key);
        T GetOrSet<T>(string key, Func<T> generator, int cacheTime = 60, bool absoluteExpiration = true);
        void Set(string key, object data, int cacheTime, bool absoluteExpiration = true);
        bool IsSet(string key);
        void Invalidate(string key);
    }
}
