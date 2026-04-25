using System;
using System.Text;

namespace Core.Utils.Extensions
{
    public static class ByteArrayExtension
    {
        public static string ToUtf8Preview(this byte[] data)
        {
            var preview = Encoding.UTF8.GetString(data, 0, Math.Min(data.Length, 64));
            return preview.Replace("\r", "\\r").Replace("\n", "\\n");
        }
        
        public static string ToHexString(this byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", " ");
        }
    }
}