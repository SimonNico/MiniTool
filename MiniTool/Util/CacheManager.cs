using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;

namespace MiniTool
{
    /// <summary>
    /// 缓存管理工具类
    /// </summary>
   public  class CacheManager
    {
       private static readonly object locker = new object();

       private static ObjectCache caches = MemoryCache.Default;
       /// <summary>
       /// 获取缓存
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <param name="key"></param>
       /// <returns></returns>
       public static T getCache<T>(string key) where T : class
       {
           if (caches.Contains(key)) return (T)caches[key];
           return default(T);
       }
       /// <summary>
       /// 检查是否有缓存
       /// </summary>
       /// <param name="key"></param>
       /// <returns></returns>
       public static bool isSetCache(string key)
       {
           return caches.Contains(key);
       }
       /// <summary>
       /// 设置缓存
       /// </summary>
       /// <param name="key"></param>
       /// <param name="value"></param>
       /// <param name="cacheTime"></param>
       public static void setCache(string key, object value, int cacheTime)
       {
           if (value == null) return;
           lock (locker)
           {
               CacheItemPolicy policy = new CacheItemPolicy();

               if (cacheTime != 0)
               {
                   policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromHours(cacheTime);
               }
               else
               { policy.Priority = CacheItemPriority.NotRemovable; }
               caches.Set(new CacheItem(key, value), policy);
           }
       }
       /// <summary>
       /// 获取缓存keys
       /// </summary>
       /// <returns></returns>
       public ICollection<string> getCacheKeys()
       {
           lock (locker)
           {
               IEnumerable<KeyValuePair<string, object>> items = caches.AsEnumerable();
               return items.Select(p => p.Key).ToList();
           }
       }
       /// <summary>
       /// 移除缓存
       /// </summary>
       /// <param name="key"></param>
       /// <returns></returns>
       public static bool RemoveCache(string key)
       {
           lock (locker)
           {
               if (caches.Contains(key))
               {
                   caches.Remove(key);
                   return true;
               }
               return false;
           }
       }
       /// <summary>
       /// 清空缓存
       /// </summary>
       public static void CacheClear()
       {
           lock (locker)
           {
               caches.ToList().ForEach(m => caches.Remove(m.Key));
           }
       }

    }
}
