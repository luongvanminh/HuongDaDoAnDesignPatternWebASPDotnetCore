using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvcblog.Data;
using mvcblog.Models;
using mvcblog.Controllers;
using Microsoft.Extensions.Logging;
using mvcblog.core;

namespace mvcblog.Areas.Admin.Blog.Controllers
{
    [Area ("Admin")]
    [Authorize]
    public class CategoryController : ControllerTemplateMethod {
        private readonly AppDbContext _context;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController (AppDbContext context,
            ILogger<CategoryController> logger) {
            _context = context;
            _logger = logger;

            CategorySingleton.Instance.Init(context);

            PrintInformation();
        }

        // GET: Admin/Category
        public IActionResult Index () {

            var items = CategorySingleton.Instance.listCatgegory;

            return View (items);

        }

        // GET: Admin/Category/Details/5
        public async Task<IActionResult> Details (int? id) {
            if (id == null) {
                return NotFound ();
            }

            var category = await _context.Categories
                .Include (c => c.ParentCategory)
                .FirstOrDefaultAsync (m => m.Id == id);
            if (category == null) {
                return NotFound ();
            }

            return View (category);
        }

        // GET: Admin/Category/Create
        public async Task<IActionResult> Create () {
            // ViewData["ParentId"] = new SelectList(_context.Categories, "Id", "Slug");
            var listcategory = await _context.Categories.ToListAsync ();
            listcategory.Insert (0, new Category () {
                Title = "Không có danh mục cha",
                    Id = -1
            });
            ViewData["ParentId"] = new SelectList (await GetItemsSelectCategorie(), "Id", "Title", -1);
            return View ();
        }

        async Task<IEnumerable<Category>> GetItemsSelectCategorie() {

            var items = await _context.Categories
                                .Include(c => c.CategoryChildren)
                                .Where(c => c.ParentCategory == null)
                                .ToListAsync();



            List<Category> resultitems = new List<Category>() {
                new Category() {
                    Id = -1,
                    Title = "Không có danh mục cha"
                }
            };
            Action<List<Category>, int> _ChangeTitleCategory = null;
            Action<List<Category>, int> ChangeTitleCategory =  (items, level) => {
                string prefix = String.Concat(Enumerable.Repeat("—", level));
                //foreach (var item in items) {
                //    item.Title = prefix + " " + item.Title; 
                //    resultitems.Add(item);
                //    if ((item.CategoryChildren != null) && (item.CategoryChildren.Count > 0)) {
                //        _ChangeTitleCategory(item.CategoryChildren.ToList(), level + 1);
                //    }

                //}

                IIterator iterator = new CategoryIterator(items);
                var item = iterator.First();
                while(!iterator.IsDone)
                {
                    item.Title = prefix + " " + item.Title;
                    resultitems.Add(item);
                    if ((item.CategoryChildren != null) && (item.CategoryChildren.Count > 0))
                    {
                        _ChangeTitleCategory(item.CategoryChildren.ToList(), level + 1);
                    }
                    item = iterator.Next();
                }

            };

            _ChangeTitleCategory = ChangeTitleCategory;
            ChangeTitleCategory(items, 0);

            return resultitems;
        }

        // POST: Admin/Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create ([Bind ("Id,ParentId,Title,Content,Slug")] Category category) {
            if (ModelState.IsValid) {
                if (category.ParentId.Value == -1)
                    category.ParentId = null;
                _context.Add (category);
                await _context.SaveChangesAsync ();

                //CategorySingleton.Instance.listCatgegory.Clear();
                //CategorySingleton.Instance.Init(_context);
                CategorySingleton.Instance.Update(_context);

                return RedirectToAction (nameof (Index));
            }

            

            // ViewData["ParentId"] = new SelectList(_context.Categories, "Id", "Slug", category.ParentId);
            //var listcategory = await _context.Categories.ToListAsync ();
            var listcategory = CategorySingleton.Instance.listCatgegory;
            listcategory.Insert (0, new Category () {
                Title = "Không có danh mục cha",
                    Id = -1
            });
            ViewData["ParentId"] = new SelectList (await GetItemsSelectCategorie(), "Id", "Title", category.ParentId);
            return View (category);
        }

        // GET: Admin/Category/Edit/5
        public async Task<IActionResult> Edit (int? id) {
            if (id == null) {
                return NotFound ();
            }

            var category = await _context.Categories.FindAsync (id);
            if (category == null) {
                return NotFound ();
            }
            
            ViewData["ParentId"] = new SelectList (await GetItemsSelectCategorie(), "Id", "Title", category.ParentId);

            return View (category);
        }

        // POST: Admin/Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit (int id, [Bind ("Id,ParentId,Title,Content,Slug")] Category category) {
            if (id != category.Id) {
                return NotFound ();
            }

            if (ModelState.IsValid) {
                try {
                    if (category.ParentId == -1) {
                        category.ParentId = null;
                    }
                    _context.Update (category);
                    await _context.SaveChangesAsync ();
                } catch (DbUpdateConcurrencyException) {
                    if (!CategoryExists (category.Id)) {
                        return NotFound ();
                    } else {
                        throw;
                    }
                }
                return RedirectToAction (nameof (Index));
            }
            //var listcategory = await _context.Categories.ToListAsync ();
            var listcategory = CategorySingleton.Instance.listCatgegory;
            listcategory.Insert (0, new Category () {
                Title = "Không có danh mục cha",
                    Id = -1
            });
            ViewData["ParentId"] = new SelectList (listcategory, "Id", "Title", category.ParentId);
            return View (category);
        }

        // GET: Admin/Category/Delete/5
        public async Task<IActionResult> Delete (int? id) {
            if (id == null) {
                return NotFound ();
            }

            var category = await _context.Categories
                .Include (c => c.ParentCategory)
                .FirstOrDefaultAsync (m => m.Id == id);
            if (category == null) {
                return NotFound ();
            }

            return View (category);
        }

        // POST: Admin/Category/Delete/5
        [HttpPost, ActionName ("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed (int id) {
            var category = await _context.Categories.FindAsync (id);
            _context.Categories.Remove (category);
            await _context.SaveChangesAsync ();
            return RedirectToAction (nameof (Index));
        }

        private bool CategoryExists (int id) {
            return _context.Categories.Any (e => e.Id == id);
        }

        protected override void PrintRoutes()
        {
            _logger.LogDebug($@"{GetType().Name}
                Routes:
                GET: Admin/Category
                GET: Admin/Category/Details/:id
                GET: Admin/Category/Create
                POST: Admin/Category/Create
                GET: Admin/Category/Edit/5
                POST: Admin/Category/Edit/:id
                GET: Admin/Category/Delete/:id
                POST: Admin/Category/Delete/:id
                ");
        }

        protected override void PrintDIs()
        {
            _logger.LogDebug($@"
                Dependencies:
                AppDbContext _context
                ILogger<CategoryController> _logger
                ");
        }
    }
}
