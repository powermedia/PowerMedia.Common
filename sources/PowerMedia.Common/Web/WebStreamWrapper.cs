using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using PowerMedia.Common.Threading;


///@TODO: add unit tests
namespace PowerMedia.Common.Web
{
    /// <summary>
    /// The class wraps WebStream and adds Position and Length implementations.
    /// </summary>
    public class WebStreamWrapper : Stream, IDisposable
    {
        private Stream _wrappedStream;
        private long _contentLength;
        private long _position;

        private WebStreamWrapper()
        {
        }

        public static WebStreamWrapper Get(Uri uri)
        {
            return Get(uri, Timeout.Infinite);
        }

        public static WebStreamWrapper Get(Uri uri, TimeSpan timeout)
        {
            return Get(uri, (int)timeout.TotalMilliseconds);
        }

        ///@TODO: unit test with custom internal http server
        public static WebStreamWrapper Get(Uri uri, int timeoutInMilliseconds)
        {
            long contentLength;
            DateTime lastModified;
            var response = GetWebResponse(uri, timeoutInMilliseconds, out contentLength, out lastModified);
            var webStream = response.GetResponseStream();
            var output = new WebStreamWrapper()
            {
                _wrappedStream = webStream,
                _contentLength = contentLength,
                LastModified = lastModified
            };

            return output;
        }

        private static WebRequest PrepareRequest(Uri uri)
        {
            WebRequest request = WebRequest.Create(uri);

            if (string.IsNullOrEmpty(uri.UserInfo) == false)
            {
                NetworkCredential credential = new NetworkCredential();
                request.Credentials = credential;

                string[] userInfo = uri.UserInfo.Split(':');
                if (userInfo.GetLength(0) >= 1)
                {
                    credential.UserName = userInfo[0];
                }
                if (userInfo.GetLength(0) >= 2)
                {
                    credential.Password = userInfo[1];
                }
            }

            return request;
        }

        private static WebResponse GetWebResponse(Uri uri, int timeoutInMilliseconds, out long sizeInBytes, out DateTime lastModified)
        {
            WebRequest request = PrepareRequest(uri);
            WebResponse response = null;
			Exception innerException = null;
            DateTime? lastModifiedTemp = null;
            long? sizeInBytesTemp = null;

            try
            {
                ThreadUtils.ExecuteMethodWithTimeout(timeoutInMilliseconds, delegate()
                {
					
                    try
                    {
                    	response = (WebResponse)request.GetResponse();
                    }
                    catch(WebException exception)
                    {
                    	innerException = exception;
                    	response = null;
                    	return;//end delegate
                    }
                    switch (uri.Scheme)
                    {
                        case "ftp":
                            break;
                        case "http":
                            sizeInBytesTemp = response.ContentLength;
                            DateTime tempTime = new DateTime();
                            lastModifiedTemp = new DateTime();
                            DateTime.TryParse(response.Headers[HttpResponseHeader.LastModified], out tempTime );
                            
                            if(tempTime.Equals(DateTime.MinValue))
                            {
                            	lastModifiedTemp = tempTime;
                            }
                            break;
                        default:
                            sizeInBytesTemp = -1;
                            lastModifiedTemp = new DateTime();
                            break;
                    }
                });
            }
            catch (TimeoutException)
            {
                if (response != null)
                {
                    ((IDisposable)response).Dispose();
                }
                throw;
            }
			
            //rethrow
            if(innerException!= null)
            {
            	throw innerException;
            }
            lastModified = lastModifiedTemp.Value;
            sizeInBytes = sizeInBytesTemp.Value;

            return response;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _wrappedStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _wrappedStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _wrappedStream.Write(buffer, offset, count);
        }

        public override void Flush()
        {
            _wrappedStream.Flush();
        }

        public override bool CanSeek
        {
            get
            {
                return _wrappedStream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return _wrappedStream.CanWrite;
            }
        }

        public override bool CanRead
        {
            get
            {
                return _wrappedStream.CanRead;
            }
        }


        public override long Length
        {
            get
            {
                return _contentLength;
            }
        }

        public override long Position
        {
            get
            {
                return _position;
            }
            set
            {
                _wrappedStream.Position = value;
            }
        }

        public DateTime LastModified { get; private set; }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = _wrappedStream.Read(buffer, offset, count);
            _position += bytesRead;
            return bytesRead;
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            base.Dispose();
            
            _wrappedStream.Dispose();
        }

        #endregion
    }
}
