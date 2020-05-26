using System;

namespace De.Hochstaetter.CommandLine
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class GetOptAttribute : Attribute
    {
        public string LongName { get; set; }
        public char ShortName { get; set; }
        public object Minimum { get; set; }
        public object Maximum { get; set; }
        public string RegexPattern { get; set; }
        public string Help { get; set; }
        public string ArgumentName { get; set; }
        public object Tag { get; set; }
        public bool HasArgument { get; set; } = true;
    }
}
