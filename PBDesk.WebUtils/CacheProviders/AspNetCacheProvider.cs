using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace PBDesk.WebUtils.CacheProviders
{
    public class AspNetCacheProvider : ICacheProvider
    {
         
        public object Get(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                return HttpRuntime.Cache[key];
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
            if (absoluteExpiration)
            {
                HttpRuntime.Cache.Add(key, data, null, DateTime.Now.AddMinutes(cacheTime), Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, null);
            }
            else
            {
                HttpRuntime.Cache.Add(key, data, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(cacheTime), CacheItemPriority.AboveNormal, null);
            }

        }

        public bool IsSet(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key", "key parameter is null or empty.");
            }
            return (HttpRuntime.Cache[key] != null);
        }

        public void Invalidate(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key", "key parameter is null or empty.");
            }
            HttpRuntime.Cache.Remove(key);
        }
    }
}
