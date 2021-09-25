using System;
using mvcblog.Models;
using System.Linq;
using System.Collections.Generic;

namespace mvcblog.core
{
    public interface IIterator
    {
        Category First();
        Category Next();
        bool IsDone { get; }
        Category CurrentItem { get; }

        void ForEachItem(Action<Category> func);
    }

    public class CategoryIterator : IIterator
    {
        List<Category> _listCategory;
        int current = 0;
        int step = 1;

        public CategoryIterator(List<Category> listCategory)
        {
            _listCategory = listCategory;
        }

        public bool IsDone {
            get { return current >= _listCategory.Count; }
        }

        public Category CurrentItem => _listCategory[current];

        public Category First()
        {
            current = 0;
            if (_listCategory.Count > 0)
                return _listCategory[current];
            return null;
        }

        public Category Next()
        {
            current += step;
            if (!IsDone)
                return _listCategory[current];
            else
                return null;
        }


        public void ForEachItem(Action<Category> func)
        {
            int i = 0;
            while (i < _listCategory.Count)
            {
                func(_listCategory[i++]);
            }
        }
    }
}
