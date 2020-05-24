using System.Collections.Generic;
using De.Hochstaetter.CommandLine.Models;

namespace De.Hochstaetter.GetOptTests
{
    internal static class TestOptions
    {
        public static IEnumerable<OptionDefinition> Standard = new[]
        {
            // Option without argument
            new OptionDefinition(longName:"verbose", shortName:'v'),

            // Option with uint argument and minimum 1 (no maximum except uint.MaxValue)
            new OptionDefinition(longName:"max-workers", shortName:'W', typeof(uint), minimum: 1),

            // Option with byte argument and restricted from 0 to 4
            new OptionDefinition(longName:"debug-level", shortName:'d', typeof(byte), minimum:0, maximum:4),

            // Option with double argument and restricted from 0 to 1
            new OptionDefinition(longName:"max-cpu-share", shortName:'c', typeof(double), minimum:0, maximum:1),

            // Option with enum argument and restricted from Monday to Friday
            new OptionDefinition(longName:"work-day", shortName:'w', typeof(WeekDay), minimum:WeekDay.Monday, maximum:WeekDay.Friday),

            // Option with bool argument
            new OptionDefinition(longName:"show-minor-errors", shortName:'e', typeof(bool)),

            // Option with string argument
            new OptionDefinition(longName:"log-file", shortName:'l', typeof(string)),

            // Option with string argument that has no short form
            new OptionDefinition(longName:"first-name", shortName:default, typeof(string)),

            // Options with string argument and restricted to a valid host name
            new OptionDefinition(longName:"host-name", shortName:'H', typeof(string), regexPattern:@"^([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])(\.([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]{0,61}[a-zA-Z0-9]))*$"),
        };
    }
}
