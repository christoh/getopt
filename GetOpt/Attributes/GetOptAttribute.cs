using System;

namespace De.Hochstaetter.CommandLine.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class GetOptAttribute : Attribute
    {
        public GetOptAttribute
        (
            //string longName = null,
            //char shortName = default,
            //bool hasArgument = true,
            //object minimum = null,
            //object maximum = null,
            //string regexPattern = null,
            //object tag = null
        )
        {
            //LongName = longName;
            //ShortName = shortName;
            //Minimum = minimum;
            //Maximum = maximum;
            //RegexPattern = regexPattern;
            //Tag = tag;
            //HasArgument = hasArgument;
        }

        public string LongName { get; set; }
        public char ShortName { get; set; }
        public object Minimum { get; set; }
        public object Maximum { get; set; }
        public string RegexPattern { get; set; }
        public object Tag { get; set; }
        public bool HasArgument { get; set; } = true;
    }
}
