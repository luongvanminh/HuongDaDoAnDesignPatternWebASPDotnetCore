using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using mvcblog.Data;
using mvcblog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using mvcblog.core;
using mvcblog.Controllers.ViewPostControllerFacade;

namespace mvcblog.Controllers {
    [Route ("/posts")]
    public class ViewPostController : Controller {

        ViewPostFacade facade;

        // Số bài hiện thị viết trên một trang danh mục
        public const int ITEMS_PER_PAGE = 4;

        public ViewPostController (ILogger<ViewPostController> logger,
            AppDbContext context,
            IMemoryCache cache) {

            CategorySingleton.Instance.Init(context);

            facade = new ViewPostFacade(logger, context, cache);

            facade.PrintRoutes();
        }

        /// Lấy danh các Categories - có dùng cache
        [NonAction]
        List<Category> GetCategories () {

            List<Category> categories;

            string keycacheCategories = "_listallcategories";

            // Phục hồi categories từ Memory cache, không có thì truy vấn Db
            if (!facade.TryGetValue (keycacheCategories, out categories)) {
                categories = facade.GetListCategoryParentIsNull();
                facade.SetCache(keycacheCategories, categories);
            }

            return categories;
        }

        // Tìm (đệ quy) trong cây, một Category theo Slug
        [NonAction]
        Category FindCategoryBySlug (List<Category> categories, string Slug) {

            //foreach (var c in categories)
            //{
            //    if (c.Slug == Slug) return c;
            //    var c1 = FindCategoryBySlug(c.CategoryChildren.ToList(), Slug);
            //    if (c1 != null)
            //        return c1;
            //}
            IIterator iterator = new CategoryIterator(categories);
            var item = iterator.First();
            while (!iterator.IsDone)
            {
                if (item.Slug == Slug) return item;
                var c1 = FindCategoryBySlug(item.CategoryChildren.ToList(), Slug);
                if (c1 != null)
                    return c1;
                item = iterator.Next();
            }
            

            return null;
        }

        [Route ("{slug?}", Name = "listpost")]
        public async Task<IActionResult> Index ([Bind (Prefix = "page")] int pageNumber, [FromRoute (Name = "slug")] string slugCategory) {

            var categories = GetCategories ();

            Category category = null;
            if (!string.IsNullOrEmpty (slugCategory)) {

                category = FindCategoryBySlug (categories, slugCategory);
                if (category == null) {
                    return NotFound ("Không thấy Category");
                }

            }
            ViewData["categories"] = categories;
            ViewData["slugCategory"] = slugCategory;
            ViewData["CurrentCategory"] = category;



            // Truy vấn lấy các post
            var posts = facade.GetAllPosts();

            if (category != null) {

                var ids = category.ChildCategoryIDs ();
                ids.Add (category.Id);

                // Lọc các Post có trong category (và con của nó)
                posts = posts.Where (p => p.PostCategories
                    .Where (c => ids.Contains (c.CategoryID)).Any ());

            }
            // Lấy tổng số dòng dữ liệu
            var totalItems = posts.Count ();
            // Tính số trang hiện thị (mỗi trang hiện thị ITEMS_PER_PAGE mục)
            int totalPages = (int) Math.Ceiling ((double) totalItems / ITEMS_PER_PAGE);
            if (totalPages < 1) totalPages = 1;
            if (pageNumber == 0) pageNumber = 1;

            if (pageNumber > totalPages) {
                var vals = new Dictionary<string, string> () { { "slug", slugCategory }};
                if (totalPages > 1) 
                    vals["page"] = totalPages.ToString ();
                return RedirectToRoute ("listpost", vals);
            }


            // Chỉ lấy các Post trang hiện tại (theo pageNumber)
            posts = posts
                .Skip (ITEMS_PER_PAGE * (pageNumber - 1))
                .Take (ITEMS_PER_PAGE)
                .OrderByDescending (p => p.DateUpdated);

            ViewData["pageNumber"] = pageNumber;
            ViewData["totalPages"] = totalPages;

            // Thực hiện truy vấn lấy List các Post và chuyển cho View
            return View (await posts.ToListAsync());
        }

        [Route ("{slug}.html", Name = "viewonepost")]
        public async Task<IActionResult> DisplayPost () {

            string Slug = (string) Request.RouteValues["slug"];

            if (string.IsNullOrEmpty (Slug)) {
                return NotFound("Không thấy trang");
            }

            // Truy vấn lấy bài viết theo Slug
            var post = await facade.GetPostBySlug(Slug);

            if (post == null) {
                return NotFound ("Không thấy trang");
            }

            var categories = GetCategories ();
            ViewData["categories"] = categories;

            return View (post);
        }

    }
}