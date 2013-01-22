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
        public uint? MaxRecords;
        public uint? CurrentPageNumber;
    }

    public class Paginator<T>
    {
        private static readonly uint DEFAULT_ITEMS_PER_PAGE_LIMIT = 20;
        private static readonly uint MAX_RECORDS = 600;
        
        private IEnumerable<T> _collection;
        private PaginationSettings _settings;

        protected Paginator(PaginationSettings settings)
        {
            _collection = null;
            _settings = settings;
        }

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

        protected uint ItemsPerPageLimit
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

        protected uint MaxRecords
        {
            get
            {
                if (_settings.MaxRecords == null)
                    return MAX_RECORDS;
                else
                    return _settings.MaxRecords.Value;
            }
        }

        protected virtual uint CalculateTotalItemsCount()
        {
            return (uint)_collection.Count();
        }

        private uint? _totalItemsCount;
        private uint TotalItemsCount
        {
            get
            {
                lock (this)
                {
                    if (_totalItemsCount == null)
                    {
                        _totalItemsCount = CalculateTotalItemsCount();
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
                        if (MaxRecords == 0)
                            _totalPageCount = (uint)Math.Ceiling((double)TotalItemsCount / (double)ItemsPerPageLimit);
                        else
                        {
                            uint total = (uint)Math.Ceiling((double)TotalItemsCount / (double)ItemsPerPageLimit);
                            uint fromParam = (uint)Math.Ceiling((double)MaxRecords / (double)ItemsPerPageLimit);
                            _totalPageCount = Math.Min(total, fromParam);
                            }
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
                if (_settings.CurrentPageNumber.HasValue && _settings.CurrentPageNumber > 1) { pageNumberCandidate = _settings.CurrentPageNumber.Value; }

                //normalize in case some items gone missing between requests
                pageNumberCandidate = Math.Min(pageNumberCandidate, TotalPageCount);
                if (pageNumberCandidate == 0)
                    return 1;
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

        protected uint SkipItemsNumber
        {
            get
            {
                return ItemsPerPageLimit * (CurrentPageNumber - 1);
            }
        }

        protected uint TakeItemsNumber
        {
            get
            {
                if (CurrentPageNumber == 1)
                {
                    if (MaxRecords == 0)
                        return ItemsPerPageLimit;
                    else
                        return Math.Min(ItemsPerPageLimit, MaxRecords);
                }
                uint skippedItems = (CurrentPageNumber - 1) * ItemsPerPageLimit;
                if (MaxRecords <= skippedItems)
                    return ItemsPerPageLimit;
                uint itemsLeft = MaxRecords - skippedItems;
                if (itemsLeft <= ItemsPerPageLimit)
                    return itemsLeft;
                return ItemsPerPageLimit;
            }
        }

        protected virtual IEnumerable<T> CalculateItemsOnCurrentPage()
        {
            IEnumerable<T> items = new List<T>();
            if (ItemsPerPageLimit == 0)
            {
                items = _collection;
            }
            else
            {
                items = _collection.Skip((int)SkipItemsNumber).Take((int)TakeItemsNumber);
                items = items.ToList();
            }
            return items;
        }

        private IEnumerable<T> _itemsOnCurrentPage;
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
                        _itemsOnCurrentPage = CalculateItemsOnCurrentPage();
                    }
                }
                return _itemsOnCurrentPage;
            }
        }
    }
}