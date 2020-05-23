using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace De.Hochstaetter.CommandLine.Models
{
    public class Parameters
    {
        public Parameters
        (
            ParseFlags options = ParseFlags.Default,
            CultureInfo culture = null,
            IEnumerable<string> trueArguments = null,
            IEnumerable<string> falseArguments = null
        )
        {
            Culture = culture ?? CultureInfo.CurrentCulture;
            Options = options;
            TrueArguments = trueArguments ?? DefaultTrueArguments;
            FalseArguments = falseArguments ?? DefaultFalseArguments;

            if ((options & ParseFlags.CaseInsensitive) != ParseFlags.Default)
            {
                TrueArguments = TrueArguments.Select(a => a.ToUpper(Culture)).ToArray();
                FalseArguments = FalseArguments.Select(a => a.ToUpper(Culture)).ToArray();
            }
        }

        public static IEnumerable<string> DefaultTrueArguments { get; } = new[] { "true", "yes", "on", "1" };
        public static IEnumerable<string> DefaultFalseArguments { get; } = new[] { "false", "no", "off", "0" };
        public IEnumerable<string> TrueArguments { get; }
        public IEnumerable<string> FalseArguments { get; }
        public ParseFlags Options { get; }
        public CultureInfo Culture { get; }
    }
}
