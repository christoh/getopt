using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using De.Hochstaetter.CommandLine.Exceptions;
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

        public static ParsedArguments Parse(IEnumerable<string> arguments, IEnumerable<OptionDefinition> optionDefinitions, Parameters parameters = null)
        {
            return new GetOpt(optionDefinitions, parameters).Parse(arguments);
        }

        public static ParsedArguments Parse(IEnumerable<OptionDefinition> optionDefinitions, Parameters parameters, params string[] arguments)
        {
            return new GetOpt(optionDefinitions, parameters).Parse(arguments);
        }

        public ParsedArguments Parse(IEnumerable<string> arguments)
        {
            if (arguments is null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            var argumentList = arguments as IList<string> ?? arguments.ToArray();

            var result = new ParsedArguments();
            var noMoreOptions = false;

            for (var i = 0; i < argumentList.Count; i++)
            {
                var argument = argumentList[i];
                var next = i < argumentList.Count - 1 && !argumentList[i + 1].StartsWith("-") ? argumentList[i + 1] : null;

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
                    var option = ParseLongOption(argument, next, ref i);
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
                var optionDefinition = optionDefinitions.SingleOrDefault
                (
                    o => o.ShortName != default && o.ShortName == argument[j]
                );

                if (optionDefinition == null)
                {
                    throw new GetOptException(GetOptError.UnknownOption, parameters, null, false, unknownOption: $"-{argument[j]}");
                }

                if (optionDefinition.HasArgument && argument.Substring(j).Length == 1 && next == null)
                {
                    throw new GetOptException(GetOptError.MustHaveArgument, parameters, optionDefinition, false);
                }

                var option = new Option { Definition = optionDefinition };

                if (optionDefinition.HasArgument)
                {
                    var isArgumentOnNextString = argument.Substring(j).Length == 1;

                    option.Argument = ConvertToTargetType
                    (
                        (isArgumentOnNextString ? next : argument.Substring(j + 1)),
                        optionDefinition,
                        false,
                        parameters.Culture
                    );

                    result.Options.Add(option);
                    if (isArgumentOnNextString) { i++; }
                    break;
                }

                result.Options.Add(option);
            }
        }

        private Option ParseLongOption(string argument, string next, ref int i)
        {
            IReadOnlyList<string> split = argument.Substring(2).Split(new[] { '=' }, 2);

            var optionDefinition = optionDefinitions.SingleOrDefault
            (
                o => o.LongName != default && o.LongName == split[0]
            );

            if (optionDefinition == null)
            {
                throw new GetOptException(GetOptError.UnknownOption, parameters, null, true, split.Count == 2 ? split[1] : null, split[0]);
            }

            switch (split.Count)
            {
                case 1 when optionDefinition.HasArgument && next == null:
                    throw new GetOptException(GetOptError.MustHaveArgument, parameters, optionDefinition, true);

                case 2 when !optionDefinition.HasArgument:
                    throw new GetOptException(GetOptError.MustNotHaveArgument, parameters, optionDefinition, true, split[1]);

                default:
                    var option = new Option { Definition = optionDefinition };

                    if (optionDefinition.HasArgument)
                    {
                        option.Argument = ConvertToTargetType
                        (
                            split.Count == 2 ? split[1] : next,
                            optionDefinition,
                            true,
                            parameters.Culture
                        );

                        i += 2 - split.Count;
                    }

                    return option;
            }
        }

        private dynamic ConvertToTargetType(string stringArgument, OptionDefinition optionDefinition, bool isLongOption, IFormatProvider culture)
        {

            dynamic argument = null;

            try
            {
                if (optionDefinition.ArgumentType.IsAssignableFrom(typeof(bool)))
                {
                    if (parameters.Options.CaseInsensitiveBoolAndEnums())
                    {
                        stringArgument = stringArgument.ToUpper(parameters.Culture);
                    }

                    if (parameters.TrueArguments.Contains(stringArgument)) { return true; }
                    if (parameters.FalseArguments.Contains(stringArgument)) { return false; }

                    Throw(GetOptError.TypeMismatch);
                }
                else if (typeof(Enum).IsAssignableFrom(optionDefinition.ArgumentType))
                {
                    argument = Enum.Parse(optionDefinition.ArgumentType, stringArgument, parameters.Options.CaseInsensitiveBoolAndEnums());
                }
                else
                {
                    argument = Convert.ChangeType(stringArgument, optionDefinition.ArgumentType, culture);
                }
            }
            catch (Exception e)
            {
                Throw(GetOptError.TypeMismatch, e);
            }

            if
            (
                (!(optionDefinition.Minimum is null) && argument < optionDefinition.Minimum) ||
                (!(optionDefinition.Maximum is null) && argument > optionDefinition.Maximum)
            )
            {
                Throw(GetOptError.OutOfRange);
            }

            if
            (
                optionDefinition.RegexPattern != null &&
                !Regex.IsMatch(stringArgument, optionDefinition.RegexPattern, parameters.RegexOptions)
            )
            {
                Throw(GetOptError.RegexFail);
            }

            return argument;

            void Throw(GetOptError error, Exception innerException = null)
            {
                throw new GetOptException(error, parameters, optionDefinition, isLongOption, stringArgument, argument, innerException: innerException);
            }
        }
    }
}
