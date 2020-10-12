using Microsoft.Extensions.Caching.Memory;

namespace SharedModels
{
    public class MemCacheWithEvent<TKey>
    {
        public MemoryCache Cache { get; }

        public MemCacheWithEvent()
        {
            this.Cache = new MemoryCache(new MemoryCacheOptions());
        }

        public void AddOrUpdateItem(TKey key, object item)
        {            
            this.Cache.Set(key, item);
        }

        public T GetItemByType<T>(TKey key)
        {
            this.Cache.TryGetValue<T>(key, out T result);
            return result;
        }

        public bool HasItem(TKey key)
        {
            return this.Cache.TryGetValue(key, out object result);
        }

        public T InvalidateItem<T>(TKey key)
        {
            if (this.Cache.TryGetValue<T>(key, out T result))
            {
                this.Cache.Remove(key);
                this.CacheInvalidation?.Invoke(this,  new CacheInvalidationEventArgs(result));
            }
            return result;
        }
        public delegate void CacheInvalidationEventHandler(object sender, CacheInvalidationEventArgs e);
        public event CacheInvalidationEventHandler CacheInvalidation;


    }
    public class CacheInvalidationEventArgs
    {
        public CacheInvalidationEventArgs(object expiredItem) { ExpiredItem = expiredItem; }
        public object ExpiredItem { get; }
    }

}
