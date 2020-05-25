using System;

namespace De.Hochstaetter.CommandLine.Models
{
    [Flags]
    public enum ParseFlags
    {
        Default = 0,
        CaseInsensitiveBoolAndEnums = 1 << 0,
    }

    public static class ParseFlagExtensions
    {
        public static bool CaseInsensitiveBoolAndEnums(this ParseFlags parseFlags)
        {
            return (parseFlags & ParseFlags.CaseInsensitiveBoolAndEnums) != ParseFlags.Default;
        }
    }
}
