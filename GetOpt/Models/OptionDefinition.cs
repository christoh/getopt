using System;
using System.Text.RegularExpressions;
using De.Hochstaetter.CommandLine.Exceptions;

namespace De.Hochstaetter.CommandLine.Models
{
    public delegate bool Validator(string stringArgument, object argument);

    /// <summary>
    /// Definition for a command line option
    /// </summary>
    public class OptionDefinition
    {
        /// <summary>
        /// Creates an <see cref="OptionDefinition"/>
        /// </summary>
        /// <param name="longName">The long name for the option for instance --use-ssl</param>
        /// <param name="shortName">The short name for the option for instance -s</param>
        /// <param name="argumentType">The type that the option argument should be converted to. Set to null if the option does not have an argument.
        /// converted to. If you don't want any conversion use <see cref="string"/></param>
        /// <param name="minimum">The minimum value allowed for the option argument. Ignored if <paramref name="argumentType"/> is null.</param>
        /// <param name="maximum">The maximum value allowed for this option argument. Ignored if <paramref name="argumentType"/> is null.</param>
        /// <param name="regexPattern">An optional <see cref="Regex"/> pattern that the argument must match. Ignored if <paramref name="argumentType"/> is null.</param>
        /// <param name="setter">An optional <see cref="System.Action{dynamic}"/> which is invoked when the option is matched on the command line</param>
        /// <param name="tag">An optional tag that can be attached to the <see cref="OptionDefinition"/>.</param>
        /// <param name="help">A line of help text what this option is good for.</param>
        /// <param name="argumentName">A name for the option argument used in help. Only used if <paramref name="argumentType"/> and <param name="help"/> are not null</param>
        /// <param name="validator">An optional custom <see cref="Models.Validator"/>. A <see cref="GetOptException"/> is thrown if the <see cref="Validator"/> returns false.</param>
        public OptionDefinition
        (
            string longName = null,
            char shortName = default,
            Type argumentType = null,
            Action<dynamic> setter = null,
            dynamic minimum = null,
            dynamic maximum = null,
            string regexPattern = null,
            Validator validator = null,
            string help = null,
            string argumentName = null,
            object tag = null
        )
        {
            if (shortName == default && longName == default)
            {
                throw new ArgumentNullException(null, $"{nameof(LongName)}, {nameof(ShortName)} or both must be set");
            }

            if (longName.StartsWith("-"))
            {
                throw new ArgumentException($"{nameof(LongName)} cannot start with '-' in {nameof(OptionDefinition)}", nameof(LongName));
            }

            if (!(minimum is null) && !(maximum is null))
            {
                if (minimum > maximum)
                {
                    throw new ArgumentException($"{nameof(Maximum)} must be equal or greater to {nameof(Minimum)}");
                }
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
            RegexPattern = regexPattern;
            Setter = setter;
            Validator = validator;
            Tag = tag;
            Help = help;
            ArgumentName = argumentName;
        }

        public string LongName { get; }
        public char ShortName { get; }
        public bool HasArgument => !(ArgumentType is null);
        public Type ArgumentType { get; }
        public dynamic Minimum { get; }
        public dynamic Maximum { get; }
        public string RegexPattern { get; }
        public Action<object> Setter { get; }
        public Validator Validator { get; }
        public string Help { get; }
        public string ArgumentName { get; }
        public object Tag { get; }

#if DEBUG
        public override string ToString()
        {
            var result = string.Empty;
            result += ShortName != default ? ShortName.ToString() : string.Empty;
            result += result != string.Empty && LongName != default ? ", " : string.Empty;
            result += LongName ?? string.Empty;
            result += ArgumentType is null ? string.Empty : $": {ArgumentType.Name}";
            result += Minimum is null ? string.Empty : $", Min: {Minimum}";
            result += Maximum is null ? string.Empty : $", Max: {Maximum}";
            return result;
        }
#endif
    }
}
