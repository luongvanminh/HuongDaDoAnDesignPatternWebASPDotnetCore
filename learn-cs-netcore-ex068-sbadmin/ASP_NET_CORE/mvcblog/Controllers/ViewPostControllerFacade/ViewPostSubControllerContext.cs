using System;
using mvcblog.Data;
using System.Linq;
using mvcblog.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace mvcblog.Controllers.ViewPostControllerFacade
{
    public class ViewPostSubControllerContext
    {
        private readonly AppDbContext _context;

        public ViewPostSubControllerContext(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Post> GetAllPosts()
        {
            return _context.Posts
                .Include(p => p.Author) // Load Author cho post  
                .Include(p => p.PostCategories) // Load các Category của Post
                .ThenInclude(c => c.Category)
                .AsQueryable();
        }

        public Task<Post> GetPostBySlug(string Slug)
        {
            return _context.Posts
                .Where(p => p.Slug == Slug)
                .Include(p => p.Author)
                .Include(p => p.PostCategories)
                .ThenInclude(c => c.Category)
                .FirstOrDefaultAsync();
        }

        public List<Category> GetListCategoryParentIsNull()
        {
            return _context.Categories
                    .Include(c => c.CategoryChildren)
                    .AsEnumerable()
                    .Where(c => c.ParentCategory == null)
                    .ToList();
        }

    }
}
