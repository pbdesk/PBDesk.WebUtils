using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;
using System.Web;

namespace PBDesk.WebUtils.CacheProviders
{
    public class DotNetInMemoryCacheProvider : ICacheProvider
    {
        private ObjectCache Cache { get { return MemoryCache.Default; } }

        public object Get(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                return Cache[key];
            }
            else
            {
                throw new ArgumentNullException("key", "key parameter is null or empty.");
            }
        }

        public T GetOrSet<T>(string key, Func<T> generator, int cacheTime = 60, bool absoluteExpiration = true)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key", "key parameter is null or empty.");
            }
            T result = (T)Get(key) ;
            if (result == null)
            {
                result = generator();
                Set(key, result, cacheTime, absoluteExpiration);
            }
            return result;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="cacheTime">Cache Time in Minutes</param>
        public void Set(string key, object data, int cacheTime, bool absoluteExpiration = true)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key", "key parameter is null or empty.");
            }
            if (cacheTime < 0)
            {
                throw new ArgumentNullException("cacheTime", "cacheTime cannot be negative.");
            }
            CacheItemPolicy policy = new CacheItemPolicy();
            if (absoluteExpiration == true)
            {
                policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheTime);
            }
            else
            {
                policy.SlidingExpiration = TimeSpan.FromMinutes(cacheTime);
            }
            Cache.Add(new CacheItem(key, data), policy);
        }

        public bool IsSet(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key", "key parameter is null or empty.");
            }
            return (Cache[key] != null);
        }

        public void Invalidate(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key", "key parameter is null or empty.");
            }
            Cache.Remove(key);
        }
    }
}
