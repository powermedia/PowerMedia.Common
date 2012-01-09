using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;

using NUnit.Framework;
using PowerMedia.Common.Web;
using PowerMedia.Common.Web.Server;

namespace PowerMedia.Common.Tests.Web
{
    [TestFixture]
    [Timeout(1000)]
    public class WebStreamWrapperTests
    {
        private HTTPServer server;
        private byte[] test;
		private static int currentPortNumber=1234;


        [SetUp]
        public void Initialize()
        {
        	server = new HTTPServer("127.0.0.1",++currentPortNumber);
        	try
        	{
            	server.Start();
        	}
        	//ugly hack for windows not cleaning up sockets properly 
        	catch(SocketException)
        	{
    			Thread.Sleep(100);
    			server.Stop();
    			server = new HTTPServer("127.0.0.1",++currentPortNumber);
    			server.Start();
        	}
            test = PowerMedia.Common.RandomUtils.RandomByteArray(2048);
            server.Map(server.PathToFileRootRequest, delegate() { return new MemoryStream(test); });
        }

        [TearDown]
        public void CleanUp()
        {
            List<string> mappedPath = server.GetMappedPaths();
            foreach (string path in mappedPath)
            {
                server.RemoveMapping(path);
            }

            List<string> movedPath = server.GetMovedPaths();
            foreach (string path in movedPath)
            {
                server.RemovePathFromMovedList(path);
            }

            server.NoFoundStatus = HTTPServer.DEFAULT_NO_FOUND_STATUS;
            server.OkStatus = HTTPServer.DEFAULT_OK_STATUS;
            server.MovedStatus = HTTPServer.DEFAULT_MOVED_STATUS;
            server.AttachLastModifiedField = true;
            server.ResetAutomaticSuspendOrDisconnect();
            server.PathToFileRootRequest = HTTPServer.DEFAULT_PATH_TO_FILE_ROOT_REQUEST;
            server.LastModified = HTTPServer.DEFAULT_LAST_MODIFIED;
            server.Stop();
        }

        [Test]
        [ExpectedException(typeof(UriFormatException))]
        public void BadUri_Test()
        {
            WebStreamWrapper.Get(new Uri("http3://.0fdsfds.0.0/"));
        }

        private bool ArrayCompare<T> (T[] firstArray, T[] secondArray) where T: IEquatable<T>
        {
            long length = firstArray.LongLength;
            if (length != secondArray.LongLength)
            {
                return false;
            }
            for (long i = 0; i < length; ++i)
            {
                if (!firstArray[i].Equals(secondArray[i]))
                {
                    Console.WriteLine("Not Equal at {0}, {1}, {2}", i, firstArray[i], secondArray[i]);
                    return false;
                }
            }
            return true;
        }

        [Test]
        public void Read_Test()
        {

            byte[] result = new byte[2048]; 
        

            server.AttachLastModifiedField = true;
            
            Stream resultStream = WebStreamWrapper.Get(new Uri("http://127.0.0.1:"+currentPortNumber));
            int readed;
            int offset=0;
            do
            {
                readed = resultStream.Read(result, offset, 2048-offset);
                offset += readed;
            } while (readed != 0);
            Assert.IsTrue(ArrayCompare(test, result), "random 2 MBbyte array check output");
        }

        [Test]
        [ExpectedException(typeof(WebException))]
        public void Read_404Error_Test()
        {

            byte[] result = new byte[2048]; 
        

            server.AttachLastModifiedField = true;
            
            Stream resultStream = WebStreamWrapper.Get(new Uri("http://127.0.0.1:"+currentPortNumber+"/blablabla.sss"));
            int readed;
            int offset=0;
            do
            {
                readed = resultStream.Read(result, offset, 2048-offset);
                offset += readed;
            } while (readed != 0);
            Assert.IsTrue(ArrayCompare(test, result), "random 2 MBbyte array check output");
        }

         [Test]
         [Timeout(3000)]
         public void Read_301Moved_Test()
         {

             byte[] result = new byte[2048];


             server.AttachLastModifiedField = true;
             server.MovePath(server.PathToFileRootRequest, "aa.htm");
             Stream resultStream = WebStreamWrapper.Get(new Uri("http://127.0.0.1:"+currentPortNumber));
             int readed;
             int offset = 0;
             do
             {
                 readed = resultStream.Read(result, offset, 2048 - offset);
                 offset += readed;
             } while (readed != 0);
             Assert.IsTrue(ArrayCompare(test, result), "random 2 MBbyte array check output");
         }

        [Test]
        public void Read_WithoutLastModifiedFieldInHTTPHeader_Test()
        {

            byte[] result = new byte[2048];

            server.AttachLastModifiedField = false;

            Stream resultStream = WebStreamWrapper.Get(new Uri("http://127.0.0.1:"+currentPortNumber));
            int readed;
            int offset = 0;
            do
            {
                readed = resultStream.Read(result, offset, 2048 - offset);
                offset += readed;
            } while (readed != 0);
            server.Stop();
            Assert.IsTrue(ArrayCompare(test, result), "random 2 MBbyte array check output");
        }

        [Test]
        [Timeout(3000)]
        public void CanSeek_Test()
        {
            byte[] result = new byte[2048];
            server.AttachLastModifiedField = true;

            Stream resultStream = WebStreamWrapper.Get(new Uri("http://127.0.0.1:"+currentPortNumber));
            Assert.IsFalse(resultStream.CanSeek);
        }

        [Test]
        public void CanSeek_Without_LastModifiedField_Test()
        {
            byte[] result = new byte[2048];
            server.AttachLastModifiedField = false;

            Stream resultStream = WebStreamWrapper.Get(new Uri("http://127.0.0.1:"+currentPortNumber));
            Assert.IsFalse(resultStream.CanSeek);
        }

        [Test]
        public void Get_Without_LastModifiedField_Test()
        {


            server.AttachLastModifiedField = false;

            Stream resultStream = WebStreamWrapper.Get(new Uri("http://127.0.0.1:"+currentPortNumber));
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Seek_Test()
        {

            byte[] result = new byte[2048];

            server.AttachLastModifiedField = true;

            Stream resultStream = WebStreamWrapper.Get(new Uri("http://127.0.0.1:"+currentPortNumber));
            resultStream.Seek(2, SeekOrigin.Begin);
        }

        [Test]
        public void CanWrite_Test()
        {

            byte[] result = new byte[2048];

            server.AttachLastModifiedField = true;

            Stream resultStream = WebStreamWrapper.Get(new Uri("http://127.0.0.1:"+currentPortNumber));
            Assert.IsFalse(resultStream.CanWrite);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Write_Test()
        {

            byte[] result = new byte[2048];

            server.AttachLastModifiedField = true;

            Stream resultStream = WebStreamWrapper.Get(new Uri("http://127.0.0.1:"+currentPortNumber));
            resultStream.Write(result, 0, 0);
        }


        [Test]
        public void Length_Test()
        {
           
            server.AttachLastModifiedField = true;

            Stream resultStream = WebStreamWrapper.Get(new Uri("http://127.0.0.1:"+currentPortNumber));
            Assert.AreEqual(2048, resultStream.Length);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void SetLength_Test()
        {

            server.AttachLastModifiedField = true;

            Stream resultStream = WebStreamWrapper.Get(new Uri("http://127.0.0.1:"+currentPortNumber));
            resultStream.SetLength(2222323);
        }

        [Test]
        public void Flush_Test()
        {

            server.AttachLastModifiedField = true;

            Stream resultStream = WebStreamWrapper.Get(new Uri("http://127.0.0.1:"+currentPortNumber));
            resultStream.Flush();
        }

        [Test]
        public void Readbyte_Test()
        {

            server.AttachLastModifiedField = true;

            Stream resultStream = WebStreamWrapper.Get(new Uri("http://127.0.0.1:"+currentPortNumber));
            Assert.AreEqual(test[0], resultStream.ReadByte());
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void WriteByte_Test()
        {

            byte[] result = new byte[2048];

            server.AttachLastModifiedField = true;

            Stream resultStream = WebStreamWrapper.Get(new Uri("http://127.0.0.1:"+currentPortNumber));
            resultStream.WriteByte(2);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WriteTimeOutSet_Test()
        {

            byte[] result = new byte[2048];

            server.AttachLastModifiedField = true;

            Stream resultStream = WebStreamWrapper.Get(new Uri("http://127.0.0.1:"+currentPortNumber));
            resultStream.WriteTimeout=12;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WriteTimeOutGet_Test()
        {

            byte[] result = new byte[2048];

            server.AttachLastModifiedField = true;

            Stream resultStream = WebStreamWrapper.Get(new Uri("http://127.0.0.1:"+currentPortNumber));
            int i=resultStream.WriteTimeout;
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void PositionSet_Test()
        {

            byte[] result = new byte[2048];

            server.AttachLastModifiedField = true;

            Stream resultStream = WebStreamWrapper.Get(new Uri("http://127.0.0.1:"+currentPortNumber));
            resultStream.Position = 12;
        }

        [Test]
        public void PositionGet_Test()
        {

            byte[] result = new byte[2048];

            server.AttachLastModifiedField = true;

            Stream resultStream = WebStreamWrapper.Get(new Uri("http://127.0.0.1:"+currentPortNumber));
            Assert.AreEqual(0, resultStream.Position);
        }



        [Test]

        public void Close_Test()
        {

            byte[] result = new byte[2048];

            server.AttachLastModifiedField = true;

            Stream resultStream = WebStreamWrapper.Get(new Uri("http://127.0.0.1:"+currentPortNumber));
            resultStream.Close();
        }

        [Test]
        [Timeout(3000)]
        public void Read_Suspended1secondConnection_Test()
        {

            byte[] result = new byte[2048];


            server.AttachLastModifiedField = true;
            server.SuspendConnectionDelayAndTime(30, TimeSpan.FromSeconds(1));
            Stream resultStream = WebStreamWrapper.Get(new Uri("http://127.0.0.1:"+currentPortNumber));

            int readed;

            int offset = 0;
            do
            {
                readed = resultStream.Read(result, offset, 2048 - offset);
                offset += readed;
            } while (readed != 0);
            Assert.IsTrue(ArrayCompare(test, result), "random 2 MBbyte array check output");
        }

        [Test]
        [ExpectedException(typeof(WebException))]
        [Timeout(3000)]
        public void Read_Disconnect_Test()
        {

            byte[] result = new byte[2048];


            server.AttachLastModifiedField = true;
            server.DisconnectDelay(129);
            Stream resultStream = WebStreamWrapper.Get(new Uri("http://127.0.0.1:"+currentPortNumber));

            int readed;

            int offset = 0;
            do
            {
                readed = resultStream.Read(result, offset, 2048 - offset);
                offset += readed;
            } while (readed != 0);
            Assert.Fail("should not get here");
        }

        [Test]
        [ExpectedException(typeof(WebException))]
        [Timeout(3000)]
        public void Read_WithTimeOut_Disconnect_Test()
        {

            byte[] result = new byte[2048];


            server.AttachLastModifiedField = true;
            server.DisconnectDelay(9);
            Stream resultStream = WebStreamWrapper.Get(new Uri("http://127.0.0.1:"+currentPortNumber), TimeSpan.FromMinutes(1));
            int readed;

            int offset = 0;
            do
            {
                readed = resultStream.Read(result, offset, 2048 - offset);
                offset += readed;
            } while (readed != 0);
            Assert.AreEqual(test[700], result[700]);
            Assert.Fail("did not disconnect during whole test");
        }


        [Test]
        [ExpectedException(typeof(WebException))]
        [Timeout(3000)]
        public void Read_WithTimeOutAsInt_Disconnect_Test()
        {

            byte[] result = new byte[2048];


            server.AttachLastModifiedField = true;
            server.DisconnectDelay(9);
            Stream resultStream = WebStreamWrapper.Get(new Uri("http://127.0.0.1:"+currentPortNumber), 60000);

            int readed;

            int offset = 0;
            do
            {
                readed = resultStream.Read(result, offset, 2048 - offset);
                offset += readed;
            } while (readed != 0);
            Assert.AreEqual(test[700], result[700]);
            Assert.Fail("did not disconnect during whole test");
        }


    }
}
