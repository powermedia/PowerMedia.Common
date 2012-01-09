using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerMedia.Common.System
{
    public static class DateUtils
    {
        public static bool DateWithoutTimeEquals(DateTime? first, DateTime? second)
        {
            bool result = false;
            if (first == null && second == null)
            {
                return true;
            }
            if (first != null && second != null)
            {
                result =(first.Value.Date == second.Value.Date);
            }
            return result;
        }
    }
}
