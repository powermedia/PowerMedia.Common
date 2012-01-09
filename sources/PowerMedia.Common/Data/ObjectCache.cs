using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections;

using PowerMedia.Common;
using PowerMedia.Common.Collections;

namespace PowerMedia.Common.Data
{
    public delegate T ObjectCreator<T>();

    public interface ICacheItem
    {
        void Refresh();
    }

    public static class GlobalCache
    {
        static object _cacheLock = new object();

        static Dictionary<Guid, ICacheItem> _cachedObjects = new Dictionary<Guid, ICacheItem>(200);
        static Dictionary<Guid, ICacheItem> CachedObjects
        {
            get
            {
                lock (_cacheLock)
                {
                    return _cachedObjects;
                }
            }
        }

        static public Guid AddItem(ICacheItem item)
        {
            lock (_cacheLock)
            {
                Guid itemId = Guid.NewGuid();
                CachedObjects[itemId] = item;
                return itemId;
            }
        }

        static public void RemoveItem(Guid itemId)
        {
            lock (_cacheLock)
            {
                CachedObjects.Remove(itemId);
            }
        }

        public static void RefreshCache()
        {
            lock (_cacheLock)
            {
                foreach (ICacheItem item in CachedObjects.Values)
                {
                    item.Refresh();
                }
            }
        }
    }

    /// <summary>
    /// Class that manages caching of an object. You can add objects of this class to the global cache by calling AddToGlobalCache() method.
    /// if you add an object to global cache you must also remember to remove it when not needed anymore (for example in IDisposable::Dispose
    /// method).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectCache<T> : ICacheItem
        where T : class
    {
        class RefreshLock : IDisposable
        {
            ObjectCache<T> _parent;
            public RefreshLock(ObjectCache<T> parent)
            {
                _parent = parent;
            }
            public void Dispose()
            {
                _parent.ReleaseRefreshLock(this);
            }
        }

        private bool _refreshRequestedDuringLock;
        public bool RefreshRequestedDuringLock
        {
            get { return _refreshRequestedDuringLock; }
            set { _refreshRequestedDuringLock = value; }
        }

        bool IsRefreshLocked
        {
            get
            {
                lock (_refreshLocksLock)
                {
                    return RefreshLocks.Count > 0;
                }
            }
        }

        object _refreshLocksLock = new object();
        HashSet<RefreshLock> _refreshLocks = new HashSet<RefreshLock>();
        HashSet<RefreshLock> RefreshLocks
        {
            get
            {
                return _refreshLocks;
            }
        }

        public IDisposable GetRefreshLock()
        {
            lock (_cachedObjectLock)
            {
                if (RefreshNeeded)
                {
                    RefreshItem();
                }
            }

            lock (_refreshLocksLock)
            {
                RefreshLock refreshLock = new RefreshLock(this);
                RefreshLocks.Add(refreshLock);
                return refreshLock;
            }
        }

        void ReleaseRefreshLock(RefreshLock refreshLock)
        {
            lock (_refreshLocksLock)
            {
                RefreshLocks.Remove(refreshLock);
                if (RefreshRequestedDuringLock)
                {
                    RefreshRequestedDuringLock = false;
                    RefreshItem();
                }
            }
        }

        object _cachedObjectLock = new object();

        ObjectCreator<T> CreateObject;
        T _cachedObject;
        public T CachedObject
        {
            get
            {
                lock (_cachedObjectLock)
                {
                    if (RefreshNeeded)
                    {
                        RefreshItem();
                    }
                    return _cachedObject;
                }
            }
        }
        
        private void RefreshItem()
        {
            lock (_cachedObjectLock)
            {
                if (IsRefreshLocked)
                {
                    RefreshRequestedDuringLock = true;
                    return;
                }

                _cachedObject = CreateObject();
                RefreshedAt = DateTime.Now;
            }
        }

        private ObjectCache(ObjectCreator<T> createObject)
            : this(createObject, DefaultRefreshInterval)
        {
        }

        private ObjectCache(ObjectCreator<T> createObject, TimeSpan refreshInterval)
        {
            CreateObject = createObject;
            _refreshInterval = refreshInterval;
            RefreshItem();
        }

        public static ObjectCache<T> Get(ObjectCreator<T> createObject)
        {
            return Get(createObject, false);
        }

        public static ObjectCache<T> Get(ObjectCreator<T> createObject, bool addToGlobalCache)
        {
            ObjectCache<T> objectCache = new ObjectCache<T>(createObject);
            if (addToGlobalCache)
            {
                objectCache.AddToGlobalCache();
            }
            return objectCache;
        }

        Guid? _globalCacheId;

        public void AddToGlobalCache()
        {
            if (_globalCacheId.HasValue == false)
            {
                _globalCacheId = GlobalCache.AddItem((ICacheItem)this);
            }
        }

        public void RemoveFromGlobalCache()
        {
            if (_globalCacheId.HasValue)
            {
                GlobalCache.RemoveItem(_globalCacheId.Value);
            }
        }

        bool RefreshNeeded
        {
            get
            {
                lock (_cachedObjectLock)
                {
                    if (RefreshedAt.HasValue == false || _cachedObject == null)
                    {
                        return true;
                    }

                    TimeSpan timeFromLastRefresh = DateTime.Now - RefreshedAt.Value;

                    return timeFromLastRefresh >= RefreshInterval;
                }
            }
        }

        readonly static TimeSpan DefaultRefreshInterval = TimeSpan.FromMinutes(15);
        readonly TimeSpan _refreshInterval;
        public TimeSpan RefreshInterval
        {
            get
            {
                return _refreshInterval;
            }
        }

        DateTime? _refreshedAt;
        DateTime? RefreshedAt
        {
            get { return _refreshedAt; }
            set { _refreshedAt = value; }
        }

        #region ICacheItem Members

        void ICacheItem.Refresh()
        {
            RefreshItem();
        }

        #endregion
    }
}
