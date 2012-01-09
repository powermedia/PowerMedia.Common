using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Diagnostics;
 namespace PowerMedia.Common.Web.Server
{
    public class HTTPServer : StatelessProtocolServer<HTTPRequest, HTTPResponse>
    {
        public const string DEFAULT_PATH_TO_FILE_ROOT_REQUEST = "index.htm";
        public const string DEFAULT_NO_FOUND_STATUS = "404 Not Found";
        public const string DEFAULT_MOVED_STATUS = "301 Moved Permanently";
        public const string DEFAULT_OK_STATUS = "200 OK";
        public const string NOT_IMPLEMENTED_STATUS = "501 Not Implemented";
        public const string INTERNAL_ERROR_STATUS = "500 Internal Server Error";


        public const string DEFAULT_SERVER_NAME = "power media test server";
        public const string DEFAULT_LAST_MODIFIED = "Wed, 08 Jan 2003 23:11:55 GMT";
        private const string ACCEPT_RANGES_SUFFIX = " bytes";
        public const string FILE_NAME_EXTENSION_SEPERATOR = ".";
        public const string PATH_DIRECTORY_SEPARATOR = "/";

        public const string NO_FOUND_MESSAGE_PREFIX = "<h2>";
        public const string NO_FOUND_MESSAGE_SUFFIX = "</h2>";
        public const string MOVED_MESSAGE_PREFIX = "<a href=\"";
        public const string MOVED_MESSAGE_SUFFIX = "\">moved here</a>";
        public const string NOT_IMPLEMENTED_MESSAGE_PREFIX = "<h2>";
        public const string NOT_IMPLEMENTED_MESSAGE_SUFFIX = "</h2>";
        public const string INTERNAL_ERROR_MESSAGE_PREFIX = "<h2>";
        public const string INTERNAL_ERROR_MESSAGE_SUFFIX = "</h2>";


        public delegate Stream StreamBuilder();
        protected Dictionary<string, StreamBuilder> _pathMapping;
        private static Dictionary<string, string> _mimeType = new Dictionary<string, string>()
            {
                {"htm", MediaTypeNames.Text.Html},
                {"html", MediaTypeNames.Text.Html},
                {"txt", MediaTypeNames.Text.Plain},
                {"xml", MediaTypeNames.Text.Xml},
                {"css", "text/css"},
                {"zip", MediaTypeNames.Application.Zip},
                {"gif", MediaTypeNames.Image.Gif},
                {"jpg", MediaTypeNames.Image.Jpeg},
                {"jpeg", MediaTypeNames.Image.Jpeg},
                {"jpe", MediaTypeNames.Image.Jpeg},
                {"png", "image/png"},
                {"ico", "image/vnd.microsoft.icon"}
            };



        
        public string ServerName { set; get;  }
        public bool AttachLastModifiedField { set; get; }
        public string LastModified { set; get; }

        public const int DEFAULT_HTTP_PORT_NUMBER = 80;

        
        public static HTTPRequest.HTTPMethods[] SupportedMethods
        {
            get
            {
                return new HTTPRequest.HTTPMethods[] { HTTPRequest.HTTPMethods.Get };
            }
        }
        public const string MESSAGE_MIME_TYPE = "text/html";
        public string PathToFileRootRequest { set; get; }
        public string NoFoundStatus { set; get; }
        public string MovedStatus { set; get; }
        public string OkStatus { set; get; }
        private enum  ErrorType { None, Suspend, Disconnect };
        private long _numberOfCorrectBytes = long.MaxValue;
        private TimeSpan _suspendTime = TimeSpan.Zero;
        private ErrorType _error = ErrorType.None;
        private Dictionary<string, string> _movedPath;


        public HTTPServer(IPAddress ip, int portNumber)
            : base(ip, portNumber)
        {
            Initialize();
        }

        public HTTPServer(string ip, int portNumber)
            : this(IPAddress.Parse(ip), portNumber)
        {
        }

        public HTTPServer(byte[] ip, int portNumber)
            : this(new IPAddress(ip), portNumber)
        {

        }

        public HTTPServer()
            : base(DEFAULT_HTTP_PORT_NUMBER)
        {
            Initialize();
        }

        private void Initialize()
        {
            LastModified = DEFAULT_LAST_MODIFIED;
            PathToFileRootRequest = DEFAULT_PATH_TO_FILE_ROOT_REQUEST;
            NoFoundStatus = DEFAULT_NO_FOUND_STATUS;
            MovedStatus = DEFAULT_MOVED_STATUS;
            OkStatus = DEFAULT_OK_STATUS;
            _movedPath = new Dictionary<string, string>();
            _pathMapping = new Dictionary<string, StreamBuilder>();
        }

        protected override HTTPRequest Read(Stream stream)
        {
            return new HTTPRequest(ReadLinesUntilDelimiter(stream, string.Empty));
        }

        private List<string> ReadLinesUntilDelimiter(Stream stream, string delimiter)
        {
            StreamReader streamReader = new StreamReader(stream);
            List<string> read = new List<string>();
            string line;
            do
            {
                line = streamReader.ReadLine();
                read.Add(line);

            }
            while (line != delimiter);
            read.RemoveAt(read.Count - 1);
            return read;
        }

        protected override void Write(Stream outStream, HTTPResponse response)
        {
            long length = response.Length;
            if (length < _numberOfCorrectBytes)
            {
                WriteFromResponse(response, outStream, length);
                return;
            }
            switch (_error)
            {
                case ErrorType.None:
                    WriteFromResponse(response, outStream, length);
                    break;

                case ErrorType.Disconnect:
                    WriteFromResponse(response, outStream, _numberOfCorrectBytes);
                    break;

                case ErrorType.Suspend:
                    WriteFromResponse(response, outStream, _numberOfCorrectBytes);
                    Thread.Sleep(_suspendTime);
                    WriteFromResponse(response, outStream, length - _numberOfCorrectBytes);
                    break;
            }
            response.Dispose();
        }

        private void WriteFromResponse(HTTPResponse response, Stream outStream, long length)
        {
            for (long i = 0; i < length; i++)
            {
                outStream.WriteByte(response.Read());
            }
        }


        protected override HTTPResponse Process(HTTPRequest request)
        {
            
            if (!SupportedMethods.Contains(request.HTTPMethod))
            {
                Trace.WriteLine(NOT_IMPLEMENTED_STATUS);
                string err = NOT_IMPLEMENTED_MESSAGE_PREFIX + NOT_IMPLEMENTED_STATUS + NOT_IMPLEMENTED_MESSAGE_SUFFIX;
                HTTPResponseHeader header = MakeHeader(request.HTTPVersion, MESSAGE_MIME_TYPE, NOT_IMPLEMENTED_STATUS);
                return new HTTPResponse(header, new MemoryStream(Encoding.ASCII.GetBytes(err)));
            }

            string path = ResolveRootPath(request.Path);
            Trace.WriteLine(path);          
            return GetHTTPResponseForPath(path, request.HTTPVersion);
            
        }

        private HTTPResponse GetHTTPResponseForPath(string path, string httpVersion)
        {
            string extension = path.Substring(path.LastIndexOf(FILE_NAME_EXTENSION_SEPERATOR) + 1);
            if (_pathMapping.ContainsKey(path))
            {
                Stream stream = _pathMapping[path].Invoke();
                HTTPResponseHeader header = MakeHeader(httpVersion, GetMimeTypeForFileExtension(extension), OkStatus);
                Trace.WriteLine(header);
                try
                {
                    return new HTTPResponse(header, stream);
                }
                catch (HTTPResponse.StreamTypeException exception)
                {
                    Trace.WriteLine(exception.Message);
                    string err = INTERNAL_ERROR_MESSAGE_PREFIX + INTERNAL_ERROR_STATUS + INTERNAL_ERROR_MESSAGE_SUFFIX;
                    HTTPResponseHeader newHeader = MakeHeader(httpVersion, MESSAGE_MIME_TYPE, INTERNAL_ERROR_STATUS);
                    Trace.WriteLine(newHeader);
                    return new HTTPResponse(newHeader, new MemoryStream(Encoding.ASCII.GetBytes(err)));
                }
            }
            else
            {
                if (_movedPath.ContainsKey(path))
                {
                    string err = MOVED_MESSAGE_PREFIX + _movedPath[path] + MOVED_MESSAGE_SUFFIX;
                    HTTPResponseHeader header = MakeHeader(httpVersion, MESSAGE_MIME_TYPE, MovedStatus, _movedPath[path]);
                    Trace.WriteLine(header);
                    return new HTTPResponse(header, new MemoryStream(Encoding.ASCII.GetBytes(err)));
                }
                else
                {
                    string err = NO_FOUND_MESSAGE_PREFIX + NoFoundStatus + NO_FOUND_MESSAGE_SUFFIX;
                    HTTPResponseHeader header = MakeHeader(httpVersion, MESSAGE_MIME_TYPE, NoFoundStatus);
                    Trace.WriteLine(header);
                    return new HTTPResponse(header, new MemoryStream(Encoding.ASCII.GetBytes(err)));

                }
            }
        }

        private string ResolveRootPath(string path)
        {
            if (path.LastIndexOf(FILE_NAME_EXTENSION_SEPERATOR) <= path.LastIndexOf(PATH_DIRECTORY_SEPARATOR))
            {
                if (path.EndsWith(PATH_DIRECTORY_SEPARATOR) || path == string.Empty)
                    return path + PathToFileRootRequest;
                else
                    return path + PATH_DIRECTORY_SEPARATOR + PathToFileRootRequest;
            }
            return path;
        }



       

        private HTTPResponseHeader MakeHeader(string httpVersion, string mimeType,  string statusCode)
        {
            HTTPResponseHeader header = new HTTPResponseHeader(httpVersion, statusCode);
            header.SetField(HTTPResponseHeader.SERVER_NAME_FIELD, ServerName);
            header.SetField(HTTPResponseHeader.CONTENT_TYPE_FIELD, mimeType);
            header.SetField(HTTPResponseHeader.ACCEPT_RANGES_FIELD, ACCEPT_RANGES_SUFFIX);
            if (AttachLastModifiedField)
            {
                header.SetField(HTTPResponseHeader.LAST_MODIFIED_FIELD, LastModified);
            }
            return header;
        }

        protected HTTPResponseHeader MakeHeader(string httpVersion, string mimeType,  string statusCode, string location)
        {
            HTTPResponseHeader header = MakeHeader(httpVersion, mimeType, statusCode);
            header.SetField(HTTPResponseHeader.LOCATION_FIELD, location);
            return header;
        }

        protected string GetMimeTypeForFileExtension(string extension)
        {
                        
            if (_mimeType.ContainsKey(extension.ToLower()))
                return _mimeType[extension.ToLower()];
            else
                return MESSAGE_MIME_TYPE;
        }

        public void Map(string path, StreamBuilder streamBuilder)
        {
            _pathMapping[path]=streamBuilder;
            _movedPath.Remove(path);
        }

        public bool RemoveMapping(string path)
        {
            return _pathMapping.Remove(path);
        }

        public bool RemovePathFromMovedList(string path)
        {
            return _movedPath.Remove(path);
        }

        public bool MovePath(string source, string destination)
        {
            if (_pathMapping.ContainsKey(source))
            {
                _pathMapping[destination] = _pathMapping[source];
                _pathMapping.Remove(source);
                _movedPath[source] = destination;
                return true;
            }
            if (_pathMapping.ContainsKey(destination))
            {
                _movedPath[source] = destination;
                return true;
            }
            return false;
        }

        public static void AddMime(string extension, string mimeType)
        {
             HTTPServer._mimeType.Add(extension, mimeType);
        }


        public void ResetAutomaticSuspendOrDisconnect()
        {
            _error = ErrorType.None;
            _numberOfCorrectBytes = long.MaxValue;
            _suspendTime = TimeSpan.Zero;
        }

        public void SuspendConnectionDelayAndTime(long numberOfCorrectBytes, TimeSpan suspendTime)
        {
            _error = ErrorType.Suspend;
            this._numberOfCorrectBytes = numberOfCorrectBytes;
            this._suspendTime = suspendTime;
        }

        public void DisconnectDelay(long numberOfCorrectBytes)
        {
            _error = ErrorType.Disconnect;
            this._numberOfCorrectBytes = numberOfCorrectBytes;
            _suspendTime = TimeSpan.Zero;
        }

        public List<string> GetMappedPaths()
        {
            return _pathMapping.Keys.ToList();
        }

        public List<string> GetMovedPaths()
        {
            return  _movedPath.Keys.ToList();
        }
    }
}
