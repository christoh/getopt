using System;

namespace De.Hochstaetter.CommandLine.Models
{
    public class OptionDefinition
    {
        public string LongName { get; }
        public char ShortName { get; }
        public bool HasArgument => ArgumentType != null;
        public Type ArgumentType { get; }
        public dynamic Minimum { get; }
        public dynamic Maximum { get; }
        public object Tag { get; }

        public OptionDefinition(string longName, char shortName = default, Type argumentType = null) : this(longName, shortName, argumentType, null, null, null) { }

        public OptionDefinition(string longName, string shortName = default, Type argumentType = null) : this(longName, StringToChar(shortName), argumentType) { }

        public OptionDefinition(string longName, string shortName, Type argumentType, object minimum = null, object maximum = null, object tag = null) :
            this(longName, StringToChar(shortName), argumentType, minimum, maximum, tag)
        {

        }

        public OptionDefinition(string longName, char shortName, Type argumentType, dynamic minimum = null, dynamic maximum = null, object tag = null)
        {
            if (shortName == default && longName == default)
            {
                throw new ArgumentNullException($"{nameof(LongName)} and {nameof(ShortName)}", $"{nameof(LongName)}, {nameof(ShortName)} or both must be set");
            }

            if (minimum != null && maximum != null && minimum > maximum)
            {
                throw new ArgumentException($"{nameof(Maximum)} must be equal or greater to {nameof(Minimum)}");
            }

            ShortName = shortName;
            LongName = longName;
            ArgumentType = argumentType;
            Maximum = maximum;
            Minimum = minimum;
            Tag = tag;
        }

        private static char StringToChar(string shortName)
        {
            if (shortName.Length != 1)
            {
                throw new ArgumentException("Short option must be one character only", nameof(shortName));
            }

            return shortName[0];
        }
    }
}
