using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;

namespace PowerMedia.Common.XML
{
    ///@TODO: unit tests
    public static class XmlParsingUtils
    {
        public static int? GetIntSafely(XmlNode node)
        {
            int retVal;

            if (NodeValidate(node) == false)
            {
                return null;
            }

            if (int.TryParse(node.InnerText, out retVal) == false)
            {
                return null;
            }

            return retVal;
        }

        public static bool NodeValidate(XmlNode node)
        {
            if (node == null)
            {
                return false;
            }

            return true;
        }

        // yyyy'-'MM'-'dd HH':'mm':'ss
        public static DateTime? GetDateSafely(XmlNode dateNode)
        {
            if (NodeValidate(dateNode) == false)
            {
                return null;
            }

            DateTime? parsedDate;

            try
            {
                parsedDate = DateTime.ParseExact(dateNode.InnerText, "yyyy'-'MM'-'dd", CultureInfo.InvariantCulture);
            }
            catch (ArgumentNullException)	{	parsedDate = null;}
            catch (FormatException)			{	parsedDate = null;}
            return parsedDate;
        }

        public static DateTime? GetDateTimeSafely(XmlNode dateNode, XmlNode timeNode)
        {
            return GetDateTimeSafely(dateNode, timeNode, "yyyy'-'MM'-'dd", "HH':'mm':'ss");
        }

        public static DateTime? GetDateTimeSafely(XmlNode dateNode, XmlNode timeNode, string dateFormat, string timeFormat)
        {
            if (NodeValidate(dateNode) == false || NodeValidate(timeNode) == false)
            {
                return null;
            }

            DateTime parsedDateTime;

            string dateTimeStr = string.Format("{0} {1}", dateNode.InnerText, timeNode.InnerText);
            string dateStr = dateNode.InnerText;
            string format = string.Format("{0} {1}", dateFormat, timeFormat);

            if (DateTime.TryParseExact(dateTimeStr, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime) == false &&
                DateTime.TryParseExact(dateStr, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime) == false)
            {
                return null;                
            }

            return parsedDateTime;
        }

        public static double? GetDoubleSafely(XmlNode node)
        {
            return (double?)GetDecimalSafely(node);
        }

        public static decimal? GetDecimalSafely(XmlNode node)
        {
            return GetMoneySafely(node, ".");
        }

        public static decimal? GetMoneySafely(XmlNode node, string decimalSeparator)
        {
            if (NodeValidate(node) == false)
            {
                return null;
            }

            decimal retVal;

            NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
            numberFormatInfo.NumberDecimalSeparator = decimalSeparator;
            NumberStyles styles = NumberStyles.AllowDecimalPoint | NumberStyles.Integer;

            if (decimal.TryParse(node.InnerText, styles, numberFormatInfo, out retVal))
            {
                return retVal;
            }

            return null;
        }
    }
}
