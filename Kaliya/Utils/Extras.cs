namespace Kaliya.Utils
{
    internal static class Extras
    {
        public static string GetDllName(string name)
        {
            var dll = $"{name}.dll";
            if (name.IndexOf(',') > 0)
            {
                dll = $"{name.Substring(0, name.IndexOf(','))}.dll";
            }

            return dll;
        }
    }
}