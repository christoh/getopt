using System;

namespace De.Hochstaetter.CommandLine.Models
{
    public class OptionDefinition
    {
        public string LongName { get; }
        public char ShortName { get; }
        public bool HasArgument => !(ArgumentType is null);
        public Type ArgumentType { get; }
        public dynamic Minimum { get; }
        public dynamic Maximum { get; }
        public object Tag { get; }

        /// <summary>
        /// Creates an option that does not have an argument
        /// </summary>
        /// <param name="longName">The long name for the option for instance --use-ssl</param>
        /// <param name="shortName">The single-character short name for the option for instance -s</param>
        public OptionDefinition(string longName, string shortName = default) : this(longName, StringToChar(shortName), null) { }

        /// <summary>
        /// Creates an option that does not have an argument
        /// </summary>
        /// <param name="longName">The long name for the option for instance --use-ssl</param>
        /// <param name="shortName">The short name for the option for instance -s</param>
        public OptionDefinition(string longName, char shortName = default) : this(longName, shortName, null) { }

        /// <summary>
        /// Creates an option that requires an argument
        /// </summary>
        /// <param name="longName">The long name for the option for instance --use-ssl</param>
        /// <param name="shortName">The single-character short name for the option for instance -s</param>
        /// <param name="argumentType">The type (e.g. <see cref="int"/>) that the option argument should be
        /// converted to. If you don't want any conversion use <see cref="string"/></param>
        /// <param name="minimum">The minimum value allowed for this option. Should be of the type as <paramref name="argumentType"/></param>
        /// <param name="maximum">The maximum value allowed for this option. Should be of the type as <paramref name="argumentType"/></param>
        public OptionDefinition(string longName, string shortName, Type argumentType, object minimum = null, object maximum = null, object tag = null) :
            this(longName, StringToChar(shortName), argumentType, minimum, maximum, tag)
        { }

        /// <summary>
        /// Creates an option that requires an argument
        /// </summary>
        /// <param name="longName">The long name for the option for instance --use-ssl</param>
        /// <param name="shortName">The short name for the option for instance -s</param>
        /// <param name="argumentType">The type (e.g. <see cref="int"/>) that the option argument should be
        /// converted to. If you don't want any conversion use <see cref="string"/></param>
        /// <param name="minimum">The minimum value allowed for this option. Should be of the type as <paramref name="argumentType"/></param>
        /// <param name="maximum">The maximum value allowed for this option. Should be of the type as <paramref name="argumentType"/></param>
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

            if (ArgumentType == null && (!(Minimum is null) || !(Maximum is null)))
            {
                throw new ArgumentException("Options without an argument can have neither minimum nor maximum");
            }

            ShortName = shortName;
            LongName = longName;
            ArgumentType = argumentType;
            Maximum = maximum;
            Minimum = minimum;
            Tag = tag;
        }

#if DEBUG
        public override string ToString()
        {
            var result = string.Empty;
            result += ShortName != default ? ShortName.ToString() : string.Empty;
            result += result != string.Empty && LongName != default ? ", " : string.Empty;
            result += LongName ?? string.Empty;
            result += ArgumentType is null ? string.Empty : $": {ArgumentType.Name}";
            result += Minimum != null ? $", Min: {Minimum}" : string.Empty;
            result += Maximum != null ? $", Max: {Maximum}" : string.Empty;
            return result;
        }
#endif

        private static char StringToChar(string shortName)
        {
            if (string.IsNullOrEmpty(shortName))
            {
                return default;
            }

            if (shortName.Length != 1)
            {
                throw new ArgumentException("Short option must have exactly one character", nameof(shortName));
            }

            return shortName[0];
        }
    }
}
