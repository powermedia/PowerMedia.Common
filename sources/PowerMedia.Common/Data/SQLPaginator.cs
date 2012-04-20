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
        private DataContext _context;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="sqlItemsQuery"> without the 'select X' part </param>
        public SQLPaginator(PaginationSettings settings, DataContext context, string sqlItemsQuery) : base(settings)
        {
            _sqlQuery = sqlItemsQuery;
            _context = context;
        }

        protected override IEnumerable<T> CalculateItemsOnCurrentPage()
        {
            var query = "select * " + _sqlQuery;
            var items =  _context.ExecuteQuery<T>(query);
            var result = items.Skip((int)SkipItemsNumber).Take((int)TakeItemsNumber).ToList(); //instantiate
            return result;
        }

        protected override uint CalculateTotalItemsCount()
        {
            var countQuery = "select count(*) " + _sqlQuery;
            return (uint)_context.ExecuteCommand(countQuery);
        }

    }
}
