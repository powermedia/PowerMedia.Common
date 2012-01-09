using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using EnterpriseDT.Net.Ftp;
namespace PowerMedia.Common.Web
{
    class FTPClientExt : FTPClient, IDisposable
    {
        void IDisposable.Dispose()
        {
            QuitImmediately();
        }

        public static FTPFile GetFileInfo(Uri uri)
        {
            using (var client = new FTPClientExt())
            {
                client.RemoteHost = uri.Host;
                client.Connect();

                if (string.IsNullOrEmpty(uri.UserInfo) == false)
                {
                    string[] parts = uri.UserInfo.Split(':');

                    if (parts.Count() > 0)
                    {
                        client.User(parts[0]);
                    }
                    if (parts.Count() > 1)
                    {
                        client.Password(parts[1]);
                    }
                }

                var directory = Path.GetDirectoryName(uri.LocalPath).Replace('\\', '/');

                client.ChDir(directory);
                var fileCollection = client.DirDetails();
                var fileName = Path.GetFileName(uri.ToString());
                var file = fileCollection.Single(f => f.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));

                return file;
            }
        }
    }
}
