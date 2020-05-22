using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using De.Hochstaetter.GetOpt.Models;

namespace De.Hochstaetter.GetOpt
{
    public static class GetOpt
    {
        public static ParsedArguments ArgumentList(this IList<string> arguments, IEnumerable<OptionDefinition> optionDefinitions, IFormatProvider culture = null)
        {
            culture = culture ?? CultureInfo.CurrentCulture;
            var optionDefinitionList = optionDefinitions as IList<OptionDefinition> ?? optionDefinitions.ToList();
            var result = new ParsedArguments();
            var noMoreOptions = false;

            for (var i = 0; i < arguments.Count; i++)
            {
                var argument = arguments[i];
                var next = i < arguments.Count - 1 && !arguments[i + 1].StartsWith("-") ? arguments[i + 1] : null;

                if (noMoreOptions)
                {
                    result.NonOptions.Add(argument);
                    continue;
                }

                if (argument == "--")
                {
                    noMoreOptions = true;
                    continue;
                }

                if (argument.StartsWith("--"))
                {
                    var option = ParseLongOption(argument, optionDefinitionList, culture);
                    result.Options.Add(option);
                    continue;
                }

                if (argument.StartsWith("-") && argument.Length > 1)
                {
                    ParseShortOption(culture, argument, optionDefinitionList, next, result, ref i);
                    continue;
                }

                result.NonOptions.Add(argument);
            }

            return result;
        }

        private static void ParseShortOption(IFormatProvider culture, string argument, IList<OptionDefinition> optionDefinitionList, string next, ParsedArguments result, ref int i)
        {
            for (var j = 1; j < argument.Length; j++)
            {
                var optionDefinition = optionDefinitionList.SingleOrDefault(o => o.ShortName != default && o.ShortName == argument[j]);

                if (optionDefinition == null)
                {
                    throw new ArgumentException($"Unknown option -{argument[j]}");
                }

                if (optionDefinition.HasArgument && argument.Substring(j).Length == 1 && next == null)
                {
                    throw new ArgumentException($"Option -{argument[j]} must have an argument");
                }

                var option = new Option { Definition = optionDefinition };

                if (optionDefinition.HasArgument)
                {
                    var isArgumentOnNextString = argument.Substring(j).Length == 1;
                    option.Argument = (argument.Substring(j).Length == 1 ? next : argument.Substring(j + 1)).ToTargetType(optionDefinition, false, culture);
                    result.Options.Add(option);
                    if (isArgumentOnNextString) { i++; }
                    break;
                }

                result.Options.Add(option);
            }
        }

        private static Option ParseLongOption(string argument, IEnumerable<OptionDefinition> optionDefinitions, IFormatProvider culture)
        {
            IReadOnlyList<string> split = argument.Substring(2).Split(new[] { '=' }, 2);
            var optionDefinition = optionDefinitions.SingleOrDefault(o => o.LongName != default && o.LongName == split[0]);

            if (optionDefinition == null)
            {
                throw new ArgumentException($"Unknown option --{split[0]}");
            }

            switch (split.Count)
            {
                case 1 when optionDefinition.HasArgument:
                    throw new ArgumentException($"Option --{split[0]} must have an argument");

                case 2 when !optionDefinition.HasArgument:
                    throw new ArgumentException($"Option --{split[0]} must not have an argument");

                default:
                    return new Option
                    {
                        Definition = optionDefinition,
                        Argument = split.Count == 2 ? split[1].ToTargetType(optionDefinition, true, culture) : null,
                    };
            }
        }

        private static object ToTargetType(this string stringArgument, OptionDefinition optionDefinition, bool isLongOption, IFormatProvider culture)
        {
            dynamic argument;
            var argumentErrorPrefix = $"Argument for option -{(isLongOption ? $"-{optionDefinition.LongName}" : $"{optionDefinition.ShortName}")} must be ";

            try
            {
                argument = Convert.ChangeType(stringArgument, optionDefinition.ArgumentType, culture);
            }
            catch (Exception e)
            {
                throw new ArgumentException($"{argumentErrorPrefix}{optionDefinition.ArgumentType.Name}", nameof(argument), e);
            }

            if (optionDefinition.Minimum != null && optionDefinition.Maximum == null && argument < optionDefinition.Minimum)
            {
                throw new ArgumentOutOfRangeException(nameof(argument), $"{argumentErrorPrefix}{optionDefinition.Minimum} or greater");
            }

            if (optionDefinition.Minimum == null && optionDefinition.Maximum != null && argument > optionDefinition.Maximum)
            {
                throw new ArgumentOutOfRangeException(nameof(argument), $"{argumentErrorPrefix}{optionDefinition.Maximum} or less");
            }

            if (optionDefinition.Minimum != null && optionDefinition.Maximum != null && (argument > optionDefinition.Maximum || argument < optionDefinition.Minimum))
            {
                throw new ArgumentOutOfRangeException(nameof(argument), $"{argumentErrorPrefix}between {optionDefinition.Minimum} and {optionDefinition.Maximum}");
            }

            return argument;
        }
    }
}
