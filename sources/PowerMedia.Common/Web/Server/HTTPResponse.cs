using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text; namespace PowerMedia.Common.Web.Server
{
    /// <summary>
    ///
    /// </summary>
    public class HTTPResponse : IDisposable
    {
        private Stream internalStream;
        private byte[] _header;
        private long _headerLength;
        private long _streamLength;
        private long _position;
        public const string READ_ERROR_MESSAGE = "Stream must be readable";
        public const string LENGTH_ERROR_MESSAGE = "Stream must implement Length property";

        public class StreamTypeException:Exception
        {
            private string _message;
            public StreamTypeException(string message)
            {
                this._message = message;
            }
            public override string Message
            {
                get
                {
                    return _message;
                }
            }
        }

        /// <summary>
        /// Return stream which at first gives bytes from header encoded in ascii
        /// and when it ends read bytes from given string
        /// </summary>
        /// <param name="header"></param>
        /// <param name="stream"></param>
        public HTTPResponse(HTTPResponseHeader header, Stream stream)
            :this (header, stream, Encoding.ASCII)
        {
        }

        /// <summary>
        /// Return stream which at first gives bytes from header encoded given encoding
        /// and when it ends read bytes from given string
        /// </summary>
        /// <param name="header"></param>
        /// <param name="stream"></param>
        public HTTPResponse(HTTPResponseHeader header, Stream stream, Encoding encoding)
        {
            try
            {
                _streamLength = stream.Length;
            }
            catch (NotSupportedException)
            {
                throw new StreamTypeException(LENGTH_ERROR_MESSAGE);
            }
            header.SetField(HTTPResponseHeader.CONTENT_LENGTH_FIELD, _streamLength.ToString());
            this._header = encoding.GetBytes(header.ToString());
            if (!stream.CanRead)
            {
                throw new StreamTypeException(READ_ERROR_MESSAGE);
            }
            internalStream = stream;
            _position = 0;
            _headerLength = this._header.LongLength;
        }





        public long Length
        {
            get 
            {
                return _headerLength + _streamLength;
            }
        }

      
        public byte Read()
        {
            if(_position < _headerLength)
            {

                return _header[_position++];              
            }
            return (byte)internalStream.ReadByte();

        }



        #region IDisposable Members

        public void Dispose()
        {
            internalStream.Close();
        }

        #endregion
    }
}
