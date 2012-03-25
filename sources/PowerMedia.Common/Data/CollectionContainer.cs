using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerMedia.Common.Data
{
    public interface CollectionContainer<ItemType>
    {
        IEnumerable<ItemType> Items { get; set; }
    }
}
