using System;
using mvcblog.Data;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using mvcblog.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcblog.Controllers.ViewPostControllerFacade
{

    public class ViewPostFacade
    {
        ViewPostSubControllerLogger vpscLogger;
        ViewPostSubControllerContext vpscContext;
        ViewPostSubControllerCache vpscCache;

        public ViewPostFacade(ILogger<ViewPostController> logger,
            AppDbContext context,
            IMemoryCache cache)
        {
            vpscLogger = new ViewPostSubControllerLogger(logger);
            vpscContext = new ViewPostSubControllerContext(context);
            vpscCache = new ViewPostSubControllerCache(cache);
        }

        public void PrintRoutes()
        {
            vpscLogger.PrintRoutes();
        }

        public bool TryGetValue(object key, out List<Category> list)
        {
            return vpscCache.TryGetValue(key, out list);
        }

        public void SetCache(object key, List<Category> list)
        {
            vpscCache.SetCache(key, list);
        }

        public IQueryable<Post> GetAllPosts()
        {
            return vpscContext.GetAllPosts();
        }

        public Task<Post> GetPostBySlug(string Slug)
        {
            return vpscContext.GetPostBySlug(Slug);
        }

        public List<Category> GetListCategoryParentIsNull()
        {
            return vpscContext.GetListCategoryParentIsNull();
        }
    }
}
