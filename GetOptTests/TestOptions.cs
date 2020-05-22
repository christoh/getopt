using System.Collections.Generic;
using De.Hochstaetter.GetOpt.Models;

namespace De.Hochstaetter.GetOptTests
{
    internal static class TestOptions
    {
        public static IEnumerable<OptionDefinition> Standard = new OptionDefinition[]
        {
            new OptionDefinition("verbose", "v"),
            new OptionDefinition("max-workers", 'W' ,typeof(int), 1, 10),
            new OptionDefinition("debug-level", 'd' ,typeof(byte), 0, 4),
            new OptionDefinition("max-cpu-share", 'c' ,typeof(double), 0, 1),
            new OptionDefinition("work-day", "w", typeof(WeekDay), WeekDay.Monday, WeekDay.Friday),
        };
    }
}
