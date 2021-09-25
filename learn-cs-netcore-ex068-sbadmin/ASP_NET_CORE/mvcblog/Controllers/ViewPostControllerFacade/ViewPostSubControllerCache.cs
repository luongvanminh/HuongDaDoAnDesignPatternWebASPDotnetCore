using System;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using mvcblog.Models;

namespace mvcblog.Controllers.ViewPostControllerFacade
{
    public class ViewPostSubControllerCache
    {
        private IMemoryCache _cache;

        public ViewPostSubControllerCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public bool TryGetValue(object key, out List<Category> list)
        {
            return _cache.TryGetValue(key, out list);
        }

        public void SetCache(object key, List<Category> list)
        {
            // Thiết lập cache - lưu vào cache
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(300));
            _cache.Set(key, list, cacheEntryOptions);
        }
    }
}
