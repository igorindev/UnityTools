using System.Text;

namespace AudioSubsystem
{
    internal static class StringToCrc32Converter
    {
        internal static uint StringToCrc32(string value)
        {
            var arrayOfBytes = Encoding.ASCII.GetBytes(value);
            var crc32 = new Crc32();
            return crc32.Get(arrayOfBytes);
        }
    }
}