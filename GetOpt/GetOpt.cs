using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using De.Hochstaetter.CommandLine.Attributes;
using De.Hochstaetter.CommandLine.Exceptions;
using De.Hochstaetter.CommandLine.Models;

namespace De.Hochstaetter.CommandLine
{
    public class GetOpt
    {
        public Parameters Parameters { get; }
        public ICollection<OptionDefinition> OptionDefinitions { get; }

        public GetOpt(IEnumerable<OptionDefinition> optionDefinitions, Parameters parameters = null)
        {
            if (optionDefinitions == null)
            {
                throw new ArgumentNullException(nameof(optionDefinitions));
            }

            OptionDefinitions = optionDefinitions as ICollection<OptionDefinition> ?? OptionDefinitions.ToArray();
            Parameters = parameters ?? Parameters.Default;
        }

        public GetOpt(object instance, Parameters parameters = null, IEnumerable<OptionDefinition> optionDefinitions = null)
            : this(GetDefinitionFromAttributes(instance, parameters, optionDefinitions), parameters)
        { }

        public static ParsedArguments Parse(IEnumerable<string> arguments, IEnumerable<OptionDefinition> optionDefinitions, Parameters parameters = null)
        {
            return new GetOpt(optionDefinitions, parameters).Parse(arguments);
        }

        public static ParsedArguments Parse(IEnumerable<OptionDefinition> optionDefinitions, Parameters parameters, params string[] arguments)
        {
            return new GetOpt(optionDefinitions, parameters).Parse(arguments);
        }

        public static ParsedArguments Parse(IEnumerable<string> arguments, object instance, Parameters parameters = null, IEnumerable<OptionDefinition> optionDefinitions = null)
        {
            return new GetOpt(instance, parameters, optionDefinitions).Parse(arguments);
        }

        public static ParsedArguments Parse(object instance, Parameters parameters = null, IEnumerable<OptionDefinition> optionDefinitions = null, params string[] arguments)
        {
            return new GetOpt(instance, parameters, optionDefinitions).Parse(arguments);
        }

        public ParsedArguments Parse(params string[] arguments) => Parse((IEnumerable<string>)arguments);

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
                    AddOption(result.Options, option);
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

        private static IEnumerable<OptionDefinition> GetDefinitionFromAttributes(object instance, Parameters parameters, IEnumerable<OptionDefinition> optionDefinitions)
        {
            IReadOnlyList<MemberInfo> members = instance.GetType()
                .GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                .Where(m => m.GetCustomAttribute<GetOptAttribute>() != null)
                .ToArray();

            var optionDefinitionCollection = optionDefinitions as ICollection<OptionDefinition> ?? new List<OptionDefinition>(members.Count);
            parameters = parameters ?? Parameters.Default;

            foreach (var member in members)
            {
                var dynamicMember = (dynamic)member;
                var attribute = member.GetCustomAttribute<GetOptAttribute>();
                Type memberType;
                Type genericArgumentType = null;

                switch (member)
                {
                    case FieldInfo fieldInfo:
                        memberType = fieldInfo.FieldType;
                        break;
                    case PropertyInfo propertyInfo:
                        memberType = propertyInfo.PropertyType;
                        break;
                    default:
                        throw new InvalidOperationException($"{nameof(GetOptAttribute)} must be used on field or property");
                }

                var collectionType = IsICollection(memberType) ? memberType : memberType.GetInterfaces().SingleOrDefault(IsICollection);

                if (collectionType != null)
                {
                    genericArgumentType = memberType.GetTypeInfo().GenericTypeArguments[0];
                }

                var optionDefinition = new OptionDefinition
                (
                    attribute.LongName, attribute.ShortName,
                    attribute.HasArgument ? (genericArgumentType ?? memberType) : null, SetValue,
                    attribute.Minimum, attribute.Maximum,
                    attribute.RegexPattern, null, attribute.Tag
                );

                optionDefinitionCollection.Add(optionDefinition);

                if (genericArgumentType != null)
                {
                    var collection = dynamicMember.GetValue(instance);

                    if (collection is null)
                    {
                        try
                        {
                            collection = Activator.CreateInstance(memberType);
                            dynamicMember.SetValue(instance, collection);
                        }
                        catch (Exception e)
                        {
                            throw new NullReferenceException($"{member.Name} must be initialized before being used with {nameof(GetOpt)}",e);
                        }
                    }
                    else
                    {
                        collection.Clear();
                    }
                }

                void SetValue(dynamic value)
                {
                    if (!attribute.HasArgument)
                    {
                        value = Convert.ChangeType(true, memberType, parameters.Culture);
                    }

                    if (!(genericArgumentType is null))
                    {
                        dynamicMember.GetValue(instance).Add(value);
                    }
                    else
                    {
                        dynamicMember.SetValue(instance, value);
                    }
                }
            }

            return optionDefinitionCollection;
        }


        private static bool IsICollection(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>);
        }

        private void ParseShortOption(string argument, string next, ParsedArguments result, ref int i)
        {
            for (var j = 1; j < argument.Length; j++)
            {
                var optionDefinition = OptionDefinitions.SingleOrDefault
                (
                    o => o.ShortName != default && o.ShortName == argument[j]
                );

                if (optionDefinition == null)
                {
                    throw new GetOptException(GetOptError.UnknownOption, Parameters, null, false, unknownOption: $"-{argument[j]}");
                }

                if (optionDefinition.HasArgument && argument.Substring(j).Length == 1 && next == null)
                {
                    throw new GetOptException(GetOptError.MustHaveArgument, Parameters, optionDefinition, false);
                }

                var option = new Option { Definition = optionDefinition };

                if (optionDefinition.HasArgument)
                {
                    var isArgumentOnNextString = argument.Substring(j).Length == 1;

                    option.Argument = GetTypedArgument
                    (
                        (isArgumentOnNextString ? next : argument.Substring(j + 1)),
                        optionDefinition,
                        false,
                        Parameters.Culture
                    );

                    AddOption(result.Options, option);
                    if (isArgumentOnNextString) { i++; }
                    break;
                }

                AddOption(result.Options, option);
            }
        }

        private static void AddOption(ICollection<Option> options, Option option)
        {
            options.Add(option);
            option.Definition.Setter?.Invoke(option.Argument);
        }

        private Option ParseLongOption(string argument, string next, ref int i)
        {
            IReadOnlyList<string> split = argument.Substring(2).Split(new[] { '=' }, 2);

            var optionDefinition = OptionDefinitions.SingleOrDefault
            (
                o => o.LongName != default && o.LongName == split[0]
            );

            if (optionDefinition == null)
            {
                throw new GetOptException(GetOptError.UnknownOption, Parameters, null, true, split.Count == 2 ? split[1] : null, split[0]);
            }

            switch (split.Count)
            {
                case 1 when optionDefinition.HasArgument && next == null:
                    throw new GetOptException(GetOptError.MustHaveArgument, Parameters, optionDefinition, true);

                case 2 when !optionDefinition.HasArgument:
                    throw new GetOptException(GetOptError.MustNotHaveArgument, Parameters, optionDefinition, true, split[1]);

                default:
                    var option = new Option { Definition = optionDefinition };

                    if (optionDefinition.HasArgument)
                    {
                        option.Argument = GetTypedArgument
                        (
                            split.Count == 2 ? split[1] : next,
                            optionDefinition,
                            true,
                            Parameters.Culture
                        );

                        i += 2 - split.Count;
                    }

                    return option;
            }
        }

        private dynamic GetTypedArgument(string stringArgument, OptionDefinition optionDefinition, bool isLongOption, IFormatProvider culture)
        {
            dynamic argument = null;

            try
            {
                if (optionDefinition.ArgumentType.IsAssignableFrom(typeof(bool)))
                {
                    if (Parameters.Options.CaseInsensitiveBoolAndEnums())
                    {
                        stringArgument = stringArgument.ToUpper(Parameters.Culture);
                    }

                    if (Parameters.TrueArguments.Contains(stringArgument)) { return true; }
                    if (Parameters.FalseArguments.Contains(stringArgument)) { return false; }

                    Throw(GetOptError.TypeMismatch);
                }
                else if (typeof(Enum).IsAssignableFrom(optionDefinition.ArgumentType))
                {
                    argument = Enum.Parse(optionDefinition.ArgumentType, stringArgument, Parameters.Options.CaseInsensitiveBoolAndEnums());
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
                !Regex.IsMatch(stringArgument, optionDefinition.RegexPattern, Parameters.RegexOptions)
            )
            {
                Throw(GetOptError.RegexFailed);
            }

            if (optionDefinition.Validator != null && !optionDefinition.Validator(stringArgument, argument))
            {
                Throw(GetOptError.CustomValidationFailed);
            }

            return argument;

            void Throw(GetOptError error, Exception innerException = null)
            {
                throw new GetOptException(error, Parameters, optionDefinition, isLongOption, stringArgument, argument, innerException: innerException);
            }
        }
    }
}
