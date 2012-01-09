using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Diagnostics; namespace PowerMedia.Common.Web.Server
{
    /// <summary>
    /// Base class for stateless protocol server
    /// </summary>
    /// <typeparam name="Request">type into which is parsed client request</typeparam>
    /// <typeparam name="Answer"></typeparam>
    public abstract class StatelessProtocolServer<RequestType, AnswerType>: TCPServer
    {
        
        protected abstract RequestType Read(Stream stream);
        protected abstract AnswerType Process(RequestType Request);
        protected abstract void Write(Stream stream, AnswerType answer);
        
        protected override void ServeClient(Stream stream)
        {
            RequestType request = Read(stream);
            Trace.WriteLine(request.ToString());
            AnswerType answer = Process(request);
            Write(stream, answer);
            stream.Close();
        }

        public StatelessProtocolServer(int portNumber)
            : base(portNumber)
        {
        }

        public StatelessProtocolServer(byte[] ip, int portNumber)
            : base(ip, portNumber)
        {
            
        }

        public StatelessProtocolServer(IPAddress ip, int portNumber)
            : base(ip, portNumber)
        {

        }
    }
}
