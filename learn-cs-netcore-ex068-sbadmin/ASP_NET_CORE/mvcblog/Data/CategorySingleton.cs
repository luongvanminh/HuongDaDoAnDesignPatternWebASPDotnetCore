using System;
using System.Linq;
using System.Collections.Generic;
using mvcblog.Models;
using Microsoft.EntityFrameworkCore;

namespace mvcblog.Data
{
    public sealed class CategorySingleton
    {
        public static CategorySingleton Instance { get; } = new CategorySingleton();
        public List<Category> listCatgegory { get; } = new List<Category>();

        private CategorySingleton() { }

        // only One time
        public void Init(AppDbContext context)
        {

            if (listCatgegory.Count == 0)
            {
                var categories = context.Categories
                    .Include(c => c.CategoryChildren)
                    .AsEnumerable()
                    .Where(c => c.ParentCategory == null)
                    .ToList();

                foreach (var item in categories)
                {
                    listCatgegory.Add(item);
                }
            }
        }

        public void Update(AppDbContext context)
        {
            listCatgegory.Clear();
            Init(context);
        }
    }
}
