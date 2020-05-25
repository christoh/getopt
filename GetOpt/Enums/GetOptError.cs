namespace De.Hochstaetter.CommandLine.Models
{
    public enum GetOptError
    {
        NoError = 0,
        TypeMismatch = 1,
        OutOfRange = 2,
        UnknownOption = 3,
        MustHaveArgument = 4,
        MustNotHaveArgument = 5,
        RegexFailed = 6,
        CustomValidationFailed = 7,
    }
}
