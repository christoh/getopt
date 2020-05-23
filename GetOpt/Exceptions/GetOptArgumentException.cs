using System;
using System.Linq;
using De.Hochstaetter.CommandLine.Models;

namespace De.Hochstaetter.CommandLine.Exceptions
{
    public class GetOptArgumentException : ArgumentException
    {
        public GetOptArgumentException
        (
            GetOptError getOptError,
            Parameters parameters,
            OptionDefinition optionDefinition,
            bool isLongOption,
            string stringArgument = null,
            object argument = null,
            string unknownOption = null,
            Exception innerException = null
        )
            : base(CreateMessage(getOptError, optionDefinition, isLongOption, stringArgument, argument, parameters, unknownOption), innerException)
        {
            OptionDefinition = optionDefinition;
            StringArgument = stringArgument;
            IsLongOption = isLongOption;
            GetOptError = getOptError;
            Argument = argument;
            Parameters = parameters;
            UnknownOption = unknownOption;
        }

        public OptionDefinition OptionDefinition { get; }
        public string StringArgument { get; }
        public string UnknownOption { get; }
        public bool IsLongOption { get; }
        public GetOptError GetOptError { get; }
        public dynamic Argument { get; }
        public Parameters Parameters { get; }

        public static string CreateMessage(GetOptError getOptError, OptionDefinition optionDefinition, bool isLongOption, string stringAargument, dynamic argument, Parameters parameters, string unknownOption)
        {
            string GetPrefix()
            {
                if (optionDefinition is null)
                {
                    return null;
                }

                var optionName = isLongOption ? $"-{optionDefinition.LongName}" : $"{optionDefinition.ShortName}";
                return $"Argument for option -{optionName} must be ";
            }

            var message = string.Empty;

            switch (getOptError)
            {
                case GetOptError.NoError:
                    message = "No error";
                    break;

                case GetOptError.UnknownOption:
                    message = $"Unknown option {unknownOption}";
                    break;

                case GetOptError.TypeMismatch:
                    if (optionDefinition.ArgumentType.IsAssignableFrom(typeof(bool)))
                    {
                        var allowedArguments = string.Join(", ", parameters.TrueArguments.Concat(parameters.FalseArguments));
                        message = $"{GetPrefix()}one of the following: {allowedArguments}";
                        break;
                    }

                    message = $"{GetPrefix()}{optionDefinition.ArgumentType.Name}";
                    break;

                case GetOptError.OutOfRange:
                    if (optionDefinition.Maximum is null && !(optionDefinition.Minimum is null) && argument < optionDefinition.Minimum)
                    {
                        message = $"{GetPrefix()}{optionDefinition.Minimum} or greater";
                    }

                    if (optionDefinition.Minimum is null && !(optionDefinition.Maximum is null) && argument > optionDefinition.Maximum)
                    {
                        message = $"{GetPrefix()}{optionDefinition.Maximum} or less";
                    }

                    if (!(optionDefinition.Minimum is null) && !(optionDefinition.Maximum is null) && (argument > optionDefinition.Maximum || argument < optionDefinition.Minimum))
                    {
                        message = $"{GetPrefix()}between {optionDefinition.Minimum} and {optionDefinition.Maximum}";
                    }

                    break;
            }
            return message;
        }
    }
}
