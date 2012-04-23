using System;
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
        public SQLPaginator(PaginationSettings settings, DataContext context, string sqlItemsQuery, string sqlCountQuery) : base(settings)
        {
            _sqlQuery = sqlItemsQuery;
            _sqlCountQuery = sqlCountQuery;
            _context = context;
        }

        protected override IEnumerable<T> CalculateItemsOnCurrentPage()
        {
            var items =  _context.ExecuteQuery<T>(_sqlQuery);
            var result = items.Skip((int)SkipItemsNumber).Take((int)TakeItemsNumber).ToList(); //instantiate
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
