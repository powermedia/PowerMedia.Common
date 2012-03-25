using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Diagnostics;
using SystemMVC = System.Web.Mvc;
using SystemIO = System.IO;
using SystemText = System.Text;

namespace PowerMedia.Common.System.Web.Mvc
{

    public class FilesController : SystemMVC.Controller
    {

        //hash => path
        private static readonly Dictionary<string, string> paths = new Dictionary<string, string>();

        public static string RegisterFile(string localFilesystemPath)
        {
            if (SystemIO.File.Exists(localFilesystemPath) == false) { return null; }


            var pathBytes = SystemText.Encoding.UTF8.GetBytes(localFilesystemPath);
            var size = new SystemIO.FileInfo(localFilesystemPath).Length.ToString();
            var sizeBytes = SystemText.Encoding.UTF8.GetBytes(size);
            var bytes = pathBytes.Concat(sizeBytes);
            var hasher = SHA256.Create();
            var hashBytes = hasher.ComputeHash(bytes.ToArray());
            var hashStrings = hashBytes.Select((singleByte) => singleByte.ToString("X2"));

            var hash = String.Join("", hashStrings);

            if (paths.ContainsKey(hash))
            {
                //must be the same path
                var oldPath = paths[hash];
                if (false == oldPath.Equals(localFilesystemPath))
                {
                    throw new InvalidOperationException("tried to register different path for the same hash");
                }
            }

            paths[hash] = localFilesystemPath;
            return hash;
        }

        private class StreamableFileResult : SystemMVC.FileStreamResult
        {
            private Int64 _lenght;

            public StreamableFileResult(SystemIO.FileStream fileStream, string mimeType, Int64 lenght)
                : base(fileStream, mimeType)
            {
                _lenght = lenght;
            }


            protected override void WriteFile(HttpResponseBase response)
            {
                response.AddHeader("content-length", _lenght.ToString());
                response.AddHeader("Connection", "Keep-Alive");
                base.WriteFile(response);
            }
        }

        private class VideoStreamResult : StreamableFileResult
        {
            private Int64 _lenght;

            public VideoStreamResult(SystemIO.FileStream fileStream, string mimeType, Int64 lenght)
                : base(fileStream, mimeType, lenght)
            {
                _lenght = lenght;
            }


            protected override void WriteFile(HttpResponseBase response)
            {
                response.AddHeader("content-type", "video/webm");
                base.WriteFile(response);
            }
        }

        private SystemMVC.ActionResult GetActionResult(string hash, bool streaming)
        {
            var path = paths[hash];
            var extension = SystemIO.Path.GetExtension(path);
            extension = extension.Substring(1);

            var fileInfo = new SystemIO.FileInfo(path);
            var lenght = fileInfo.Length;
            
            string mimeType = null;
            if ("mp4".Equals(extension)) { mimeType = "video/mp4"; }
            if ("mpe".Equals(extension)) { mimeType = "video/mpeg"; }
            if ("mpeg".Equals(extension)) { mimeType = "video/mpeg"; }
            if ("mpg".Equals(extension)) { mimeType = "video/mpeg"; }
            if ("mov".Equals(extension)) { mimeType = "video/quicktime"; }
            if ("mxf".Equals(extension)) { mimeType = "application/mxf"; }
            if ("pdf".Equals(extension)) { mimeType = "application/pdf"; }

            if (mimeType == null)
            {
                Trace.TraceWarning("FileController: unknown file extension: " + extension + "\n sending as application/octet-stream");
                mimeType = "application/octet-stream"; 
            }

            var fileStream = SystemIO.File.OpenRead(path);
            SystemMVC.FileStreamResult result = null;
            if (streaming)
            {
                result = new VideoStreamResult(fileStream, mimeType, lenght);
            }
            else
            {
                var filename = SystemIO.Path.GetFileName(path);
                result = new SystemMVC.FileStreamResult(fileStream, mimeType);
                result.FileDownloadName = filename;
            }

            return result;
        }

        public SystemMVC.ActionResult Download(string hash)
        {
            return GetActionResult(hash, false);
        }

        private SystemMVC.ActionResult StreamContent(string hash)
        {
            return GetActionResult(hash, true);
        }

        public SystemMVC.ActionResult Stream(string hash)
        {
            return StreamContent(hash);
        }

        public SystemMVC.ActionResult Index(string hash)
        {
            return StreamContent(hash);
        }

    }
}
