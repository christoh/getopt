using System;

namespace De.Hochstaetter.CommandLine.Models
{
    [Flags]
    public enum ParseFlags
    {
        Default = 0,
        CaseInsensitive = 1 << 0,
    }
}
