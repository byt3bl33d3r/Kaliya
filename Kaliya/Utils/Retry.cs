using System;
using System.Collections.Generic;
using System.Threading;

namespace Kaliya.Utils
{
    internal static class Retry
    {
        public static T Do<T>(Func<T> action, TimeSpan retryInterval,
            int maxAttempts = 3)
        {
            var exceptions = new List<Exception>();

            for (var attempts = 0; attempts < maxAttempts; attempts++)
            {
                try
                {
                    if (attempts > 0)
                    {
                        Thread.Sleep(retryInterval);
                    }

                    return action();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            throw new AggregateException(exceptions);
        }
    }
}