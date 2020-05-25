using System;
using System.Collections.Generic;
using System.Reflection;
using De.Hochstaetter.CommandLine;
using De.Hochstaetter.CommandLine.Attributes;
using De.Hochstaetter.CommandLine.Exceptions;
using De.Hochstaetter.CommandLine.Models;

namespace De.Hochstaetter.Commandline.Demo
{
    internal class Program
    {
        private const string VerboseHelp = "Show verbose output";
        private const string LogFileHelp = "Write log to <file>";
        private const string NotifyHelp = "Send notification to <email-address>. Can be used multiple times.";
        private const string HelpHelp = "Show this help";
        private const string WorkDayHelp = "Day of week when your job should run";

        private const string JobTagHelp = "Add job tag <job-tag>. Max 8 chars, upper-case letter, numeric digits and underscores only." +
                                          "\n\tFirst char must be upper-case letter. Can be used multiple times.";

        [GetOpt(LongName = "verbose", ShortName = 'v', HasArgument = false, Help = VerboseHelp)]
        public static bool Verbose;

        [GetOpt(LongName = "help", ShortName = 'h', HasArgument = false, Help = HelpHelp)]
        public static bool ShowHelp;

        [GetOpt(LongName = "log-file", ShortName = 'l', ArgumentName = "<file>", Help = LogFileHelp)]
        public static string LogFile { get; set; }

        [GetOpt(LongName = "notify-email", ShortName = 'n', ArgumentName = "<email-address>", Help = NotifyHelp)]
        public static ISet<string> EmailNotifications { get; } = new HashSet<string>();

        [GetOpt(LongName = "work-day", ShortName = 'w', Minimum = DayOfWeek.Monday, Maximum = DayOfWeek.Friday, ArgumentName = "<Monday..Friday>", Help = WorkDayHelp)]
        private static DayOfWeek? WorkDay;

        [GetOpt(LongName = "tag", ShortName = 't', RegexPattern = "^[A-Z][A-Z0-9_]{0,7}$", ArgumentName = "<job-tag>", Help = JobTagHelp)]
        public static List<string> JobTags { get; set; }

        internal static void Main(string[] args)
        {
            Console.WriteLine($"demo - a program that has many command line options - version {Assembly.GetExecutingAssembly().GetName().Version}");
            Console.WriteLine();
            Console.WriteLine($"Command line: {Environment.CommandLine}");
            Console.WriteLine();

            var getOpt = new GetOpt(typeof(Program));
            ParsedArguments parsedArguments;

            try
            {
                parsedArguments = getOpt.Parse(args);
            }
            catch (GetOptException ex)
            {
                ShowExceptionAndExit(ex);
                return;
            }

            if (ShowHelp)
            {
                Usage(getOpt);
                return;
            }

            Console.WriteLine("Job files to run     : {0}", string.Join(", ", parsedArguments.NonOptions));
            Console.WriteLine("Run job              : {0}", WorkDay.HasValue ? (object)WorkDay.Value : "ASAP");
            Console.WriteLine("Log goes to to       : {0}", LogFile);
            Console.WriteLine("Be verbose           : {0}", Verbose ? "yes" : "no");
            Console.WriteLine("Send notification to : {0}", string.Join(", ", EmailNotifications));
            Console.WriteLine("Job tags             : {0}", string.Join(", ", JobTags));
        }

        private static void Usage(GetOpt getOpt)
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("\tdemo [ <option>... ] <job-file>...");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine();
            Console.WriteLine(getOpt.GetHelp());
            Console.WriteLine();
            Environment.Exit(0);
        }

        private static void ShowExceptionAndExit(GetOptException ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            Console.Error.WriteLine("Try -h or --help for proper usage.");
            Console.Error.WriteLine();
            Environment.Exit(87);
        }
    }
}
