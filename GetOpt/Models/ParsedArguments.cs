using System.Collections.Generic;

namespace De.Hochstaetter.CommandLine.Models
{
    public class ParsedArguments
    {
        public IList<Option> Options { get; } = new List<Option>();
        public IList<string> NonOptions { get; } = new List<string>();
    }
}
