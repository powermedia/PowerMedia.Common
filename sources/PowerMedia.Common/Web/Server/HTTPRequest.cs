using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; namespace PowerMedia.Common.Web.Server
{
    public class HTTPRequest
    {
        public enum HTTPMethods {Get, Head, Post, Put, Delete, Trace, Option, Connect, Other};
        public HTTPMethods HTTPMethod { get; private set; }
        public string HTTPVersion { get; private set; }
        public string Path { get; private set; }
        private static Dictionary<string, HTTPMethods> _httpMethods = new Dictionary<string,HTTPMethods> 
        {
            {"GET", HTTPMethods.Get},
            {"HEAD", HTTPMethods.Head},
            {"POST", HTTPMethods.Post},
            {"PUT", HTTPMethods.Put},
            {"DELETE", HTTPMethods.Delete},
            {"TRACE", HTTPMethods.Trace},
            {"OPTION", HTTPMethods.Option},
            {"Connect", HTTPMethods.Connect}

        };

        private Dictionary<string, string> _httpFields;

        public const int METHOD_IN_HEADER_POSITION = 0;
        public const int PATH_IN_HEADER_POSITION = 1;
        public const int PROTOCOL_IN_HEADER_POSITION = 2;
        public const int STRING_BEGINNING = 0;
        public const char PATH_DIRECTORY_SEPARATOR = '/';

        public HTTPRequest(List<string> request)
        {
            _httpFields = new Dictionary<string, string>();
            string[] requestHeaderTokens = request.First().Split(' ');
            HTTPVersion = requestHeaderTokens[PROTOCOL_IN_HEADER_POSITION];
            Path = requestHeaderTokens[PATH_IN_HEADER_POSITION].Trim();
            if (Path[0] == PATH_DIRECTORY_SEPARATOR)
            {
                Path = Path.Substring(1);
            }
            string httpMethod = requestHeaderTokens[METHOD_IN_HEADER_POSITION];
            if (_httpMethods.ContainsKey(httpMethod))
            {
                HTTPMethod = _httpMethods[httpMethod];
            }
            else
            {
                HTTPMethod = HTTPMethods.Other;
            }
            request.Remove(request.First());
            foreach (string line in request)
            {
                int colonPosition = line.IndexOf(":");
                string fieldName = line.Substring(STRING_BEGINNING, colonPosition - 1).Trim();
                string fieldValue = line.Substring(colonPosition + 1).Trim();
                _httpFields[fieldName]=fieldValue;                
            }
       }
        public string GetFieldValue(string field)
        {
            if (_httpFields.ContainsKey(field))
            {
                return _httpFields[field];
            }
            else
            {
                return string.Empty;
            }
        }
        public List<string> SettedFields()
        {
            return _httpFields.Keys.ToList();
        }
    }
}
