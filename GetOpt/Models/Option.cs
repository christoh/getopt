namespace De.Hochstaetter.CommandLine
{
    public class Option
    {
        public OptionDefinition Definition { get; set; }
        public dynamic Argument { get; set; }

#if DEBUG
        public override string ToString()
        {
            var result = string.Empty;
            result += Definition.ShortName != default ? Definition.ShortName.ToString() : string.Empty;
            result += result != string.Empty && Definition.LongName != default ? ", " : string.Empty;
            result += Definition.LongName ?? string.Empty;
            result += Argument is null ? string.Empty : $": {Argument}";
            return result;
        }
#endif
    }
}
