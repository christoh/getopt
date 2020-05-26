using System.Globalization;
using De.Hochstaetter.CommandLine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace De.Hochstaetter.GetOptTests
{
    [TestClass]
    public class ActionTests
    {
        private bool verbose;
        private string hostName;
        private WeekDay workDay;
        private double maxCpuShare;

        [TestMethod]
        public void TestActions()
        {
            var options = new[]
            {
                new OptionDefinition(longName:"verbose", shortName:'v', setter: _ => verbose = true),
                new OptionDefinition(longName:"max-workers", shortName:'W', typeof(uint), minimum: 1),
                new OptionDefinition(longName:"debug-level", shortName:'d', typeof(byte), minimum:0, maximum:4),
                new OptionDefinition(longName:"max-cpu-share", shortName:'c', typeof(double), minimum:0, maximum:1, setter: optArg => maxCpuShare = optArg),
                new OptionDefinition(longName:"work-day", shortName:'w', typeof(WeekDay), minimum:WeekDay.Monday, maximum:WeekDay.Friday, setter: optArg => workDay = optArg),
                new OptionDefinition(longName:"show-minor-errors", shortName:'e', typeof(bool)),
                new OptionDefinition(longName:"log-file", shortName:'l', typeof(string)),
                new OptionDefinition(longName:"first-name", shortName:default, typeof(string)),
                new OptionDefinition(longName:"host-name", shortName:'H', typeof(string), setter: optArg => hostName = optArg, regexPattern:@"^([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])(\.([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]{0,61}[a-zA-Z0-9]))*$"),
            };

            var commandline = new[] { "-vHwww.google.com", "--max-cpu-share=.5", "--work-day", "Friday" };
            GetOpt.Parse(commandline, options);

            Assert.IsTrue(verbose);
            Assert.AreEqual("www.google.com", hostName);
            Assert.AreEqual(WeekDay.Friday, workDay);
            Assert.AreEqual(.5, maxCpuShare, 1e-6);
        }
    }
}
