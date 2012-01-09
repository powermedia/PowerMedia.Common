using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; namespace PowerMedia.Common.Web.Server
{
    public class HTTPResponseHeader
    {
        public const string CONTENT_TYPE_FIELD = "Content-Type";
        public const string SERVER_NAME_FIELD = "Server";
        public const string ACCEPT_RANGES_FIELD = "Accept-Ranges";
        public const string CONTENT_LENGTH_FIELD = "Content-Length";
        public const string LOCATION_FIELD = "Location";
        public const string LAST_MODIFIED_FIELD = "Last-Modified";

        public const string PROTOCOL_STATUS_SEPARATOR = " ";
        public const string LINE_DELIMITER = "\r\n";
        public const string FIELD_NAME_VALUE_SEPARATOR=": ";
        public string Protocol { get; set; }
        public string Status { get; set; }
        private Dictionary<string, string> _fields;

        public HTTPResponseHeader(string protocol, string status)
        {
            Protocol = protocol;
            Status = status;
            _fields = new Dictionary<string, string>();
        }
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append(Protocol);
            result.Append(PROTOCOL_STATUS_SEPARATOR);
            result.Append(Status);
            result.Append(LINE_DELIMITER);
            foreach(KeyValuePair<string, string> kv in _fields)
            {
                result.Append(kv.Key + FIELD_NAME_VALUE_SEPARATOR + kv.Value + LINE_DELIMITER);
            }
            result.Append(LINE_DELIMITER);
            return result.ToString();
        }

        public void SetField(string field, string value)
        {
            _fields[field] = value;
        }

        public bool UnSetField(string field)
        {
            return _fields.Remove(field);
        }

        public List<string> GetSettedFields()
        {
            return _fields.Keys.ToList();
        }
    }
}
