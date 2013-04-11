using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PBDesk.WebUtils.CacheProviders
{
    public class CacheManager
    {
        private static ICacheProvider defaultCacheProvider = new DotNetInMemoryCacheProvider();

        public static ICacheProvider DefaultCacheProvider 
        {
            get
            {
                return defaultCacheProvider;
            }
            set
            {
                if (value != null)
                {
                    defaultCacheProvider = value;
                }
                else
                {
                    defaultCacheProvider = new DotNetInMemoryCacheProvider();
                }
            }
        }

        public static ICacheProvider CustomCacheProvider(ICacheProvider provider)
        {
            return provider;
        }
    }
}
