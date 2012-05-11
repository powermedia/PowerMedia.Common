﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;

namespace PowerMedia.Common.Data
{
    public class SQLPaginator<T> : Paginator<T>
    {
        private string _sqlQuery ;
        private string _sqlCountQuery;
        private DataContext _context;
        private IEnumerable<T> _items;
        Func<IEnumerable<T>, IEnumerable<T>> _filter;

        public SQLPaginator(PaginationSettings settings, DataContext context, string sqlItemsQuery, string sqlCountQuery, Func<IEnumerable<T>, IEnumerable<T>> inMemoryFilter = null) 
            : base(settings)
        {
            _sqlQuery = sqlItemsQuery;
            _sqlCountQuery = sqlCountQuery;
            _context = context;
            _filter = inMemoryFilter;
        }

        protected override IEnumerable<T> CalculateItemsOnCurrentPage()
        {
            if (_items == null)
            {
                _items =  _context.ExecuteQuery<T>(_sqlQuery);
                if (_filter != null)
                {
                    _items = _filter(_items);
                }
            }

            var result = _items.Skip((int)SkipItemsNumber).Take((int)TakeItemsNumber).ToList(); //instantiate
            return result;
        }

        protected override uint CalculateTotalItemsCount()
        {

            var counts = _context.ExecuteQuery<int>(_sqlCountQuery);
            var result = counts.First();
            return (uint)result;
        }

    }
}
