using System;

namespace De.Hochstaetter.CommandLine.Models
{
    [Flags]
    public enum ParseFlags
    {
        Default = 0,
        CaseInsensitive = 1 << 0,
    }

    public static class ParseFlagExtensions
    {
        public static bool CaseInsensitive(this ParseFlags parseFlags) => (parseFlags & ParseFlags.CaseInsensitive) != ParseFlags.Default;
    }
}
