using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerMedia.Common.Data
{
    public class ByteSize
    {
        public static string SizeFormatted(ulong size)
        {
            if (size == 0) { return "0B"; }

            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB" };
            int place = Convert.ToInt32(Math.Floor(Math.Log(size, 1024)));
            double num = Math.Round(size / Math.Pow(1024, place), 1);
            string readable = num.ToString() + suf[place];

            return readable;
        }


    }
}
