﻿using System;
using System.Collections.Generic;
using System.Linq;
using De.Hochstaetter.CommandLine.Models;

namespace De.Hochstaetter.CommandLine
{
    public class GetOpt
    {
        private readonly Parameters parameters;
        private readonly IList<OptionDefinition> optionDefinitions;

        public GetOpt(IEnumerable<OptionDefinition> optionDefinitions, Parameters parameters = null)
        {
            if (optionDefinitions == null)
            {
                throw new ArgumentNullException(nameof(optionDefinitions));
            }

            this.optionDefinitions = optionDefinitions as IList<OptionDefinition> ?? this.optionDefinitions.ToArray();
            this.parameters = parameters ?? new Parameters();
        }

        public static ParsedArguments Parse(IList<string> arguments, IEnumerable<OptionDefinition> optionDefinitions, Parameters parameters = null)
        {
            return new GetOpt(optionDefinitions, parameters).Parse(arguments);
        }

        public ParsedArguments Parse(IList<string> arguments)
        {
            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

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
                    var option = ParseLongOption(argument);
                    result.Options.Add(option);
                    continue;
                }

                if (argument.StartsWith("-") && argument.Length > 1)
                {
                    ParseShortOption(argument, next, result, ref i);
                    continue;
                }

                result.NonOptions.Add(argument);
            }

            return result;
        }

        private void ParseShortOption(string argument, string next, ParsedArguments result, ref int i)
        {
            for (var j = 1; j < argument.Length; j++)
            {
                var optionDefinition = optionDefinitions.SingleOrDefault(o => o.ShortName != default && o.ShortName == argument[j]);

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
                    option.Argument = ConvertToTargetType((argument.Substring(j).Length == 1 ? next : argument.Substring(j + 1)), optionDefinition, false, parameters.Culture);
                    result.Options.Add(option);
                    if (isArgumentOnNextString) { i++; }
                    break;
                }

                result.Options.Add(option);
            }
        }

        private Option ParseLongOption(string argument)
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
                        Argument = split.Count == 2 ? ConvertToTargetType(split[1], optionDefinition, true, parameters.Culture) : null,
                    };
            }
        }

        private object ConvertToTargetType(string stringArgument, OptionDefinition optionDefinition, bool isLongOption, IFormatProvider culture)
        {
            dynamic argument;
            var argumentErrorPrefix = $"Argument for option -{(isLongOption ? $"-{optionDefinition.LongName}" : $"{optionDefinition.ShortName}")} must be ";

            try
            {
                if (optionDefinition.ArgumentType.IsAssignableFrom(typeof(bool)))
                {
                    argument = ParseBool(stringArgument, argumentErrorPrefix);
                }
                else if (typeof(Enum).IsAssignableFrom(optionDefinition.ArgumentType))
                {
                    argument = Enum.Parse(optionDefinition.ArgumentType, stringArgument);
                }
                else
                {
                    argument = Convert.ChangeType(stringArgument, optionDefinition.ArgumentType, culture);
                }
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

        private bool ParseBool(string argument, string argumentErrorPrefix)
        {
            if ((parameters.Options & ParseFlags.CaseInsensitive) != ParseFlags.Default)
            {
                argument = argument.ToUpper(parameters.Culture);
            }

            if (parameters.TrueArguments.Contains(argument)) { return true; }
            if (parameters.FalseArguments.Contains(argument)) { return false; }
            throw new ArgumentException($"{argumentErrorPrefix}one of the following: {string.Join(", ", parameters.TrueArguments.Concat(parameters.FalseArguments))}");
        }
    }
}
