using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerMedia.Common.Data
{
    [Serializable]
    public class PaginationSettings
    {
        public uint? ItemsPerPageLimit;
        public uint? CurrentPageNumber;
    }

    public class Paginator<T>
    {
        private static readonly uint DEFAULT_ITEMS_PER_PAGE_LIMIT = 20;
        private static readonly uint MAX_NUMBER_OF_PAGES = 30;
        
        private IEnumerable<T> _collection;
        private PaginationSettings _settings;
        public Paginator(PaginationSettings settings, IEnumerable<T> collection)
        {
            if (collection == null) { throw new ArgumentNullException(); }
            if (settings == null)
            {
                settings = new PaginationSettings();
            }
            _settings = settings;
            _collection = collection;
        }

        private uint ItemsPerPageLimit
        {
            get
            {
                if (_settings.ItemsPerPageLimit == null)
                {
                    return DEFAULT_ITEMS_PER_PAGE_LIMIT;
                }
                else
                {
                    return _settings.ItemsPerPageLimit.Value;
                }
            }
        }

        uint? _totalItemsCount;
        private uint TotalItemsCount
        {
            get
            {
                lock (this)
                {
                    if (_totalItemsCount == null)
                    {
                        _totalItemsCount = (uint)_collection.Count();
                    }
                }
                return _totalItemsCount.Value;
            }
        }

        private uint? _totalPageCount;
        public uint TotalPageCount
        {
            get
            {
                lock (this)
                {
                    if (_totalPageCount == null)
                    {
                        _totalPageCount = (uint)Math.Ceiling((double)TotalItemsCount / (double)ItemsPerPageLimit);
                        _totalPageCount = Math.Min(_totalPageCount.Value, MAX_NUMBER_OF_PAGES);
                    }
                }
                return _totalPageCount.Value;
            }
        }

        /// <summary>
        /// always >= 1
        /// </summary>
        public uint CurrentPageNumber
        {
            get
            {
                uint pageNumberCandidate = 1;
                if (_settings.CurrentPageNumber.HasValue) { pageNumberCandidate = _settings.CurrentPageNumber.Value; }

                //normalize in case some items gone missing between requests
                pageNumberCandidate = Math.Min(pageNumberCandidate, TotalPageCount);
                return pageNumberCandidate;
            }
        }

        private uint FirstItemOnCurrentPageNumber
        {
            get
            {
                return SkipItemsNumber + 1;
            }
        }

        private uint SkipItemsNumber
        {
            get
            {
                return ItemsPerPageLimit * (CurrentPageNumber - 1);
            }
        }

        private uint TakeItemsNumber
        {
            get
            {
                return ItemsPerPageLimit;
            }
        }

        IEnumerable<T> _itemsOnCurrentPage;
        public IEnumerable<T> ItemsOnCurrentPage
        {
            get
            {
                if (_itemsOnCurrentPage != null)
                {
                    return _itemsOnCurrentPage;
                }
                lock (this)
                {
                    if (_itemsOnCurrentPage == null)
                    {
                        if (ItemsPerPageLimit == 0)
                        {
                            _itemsOnCurrentPage = _collection;
                        }
                        _itemsOnCurrentPage = _collection.Skip((int)SkipItemsNumber).Take((int)TakeItemsNumber);
                        _itemsOnCurrentPage = _itemsOnCurrentPage.ToList();
                    }
                }
                return _itemsOnCurrentPage;
            }
        }

    }
}