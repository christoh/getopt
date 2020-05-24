using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace De.Hochstaetter.CommandLine.Models
{
    public class Parameters
    {
        public Parameters
        (
            ParseFlags options = ParseFlags.Default,
            CultureInfo culture = null,
            IEnumerable<string> trueArguments = null,
            IEnumerable<string> falseArguments = null,
            RegexOptions regexOptions = RegexOptions.None
        )
        {
            Culture = culture ?? CultureInfo.CurrentCulture;
            Options = options;
            TrueArguments = trueArguments ?? DefaultTrueArguments;
            FalseArguments = falseArguments ?? DefaultFalseArguments;
            RegexOptions = regexOptions;

            if (options.CaseInsensitiveBoolAndEnums())
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
        public RegexOptions RegexOptions { get; }

        public static Parameters Default => new Parameters();

#if DEBUG
        public override string ToString()
        {
            return
                $"{Options}, {Culture}, " +
                $"True={string.Join("/", TrueArguments)}, " +
                $"False={string.Join("/", FalseArguments)}";
        }
# endif
    }
}
