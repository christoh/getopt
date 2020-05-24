using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using De.Hochstaetter.CommandLine;
using De.Hochstaetter.CommandLine.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace De.Hochstaetter.GetOptTests
{
    [TestClass]
    public class WorkDayTests
    {
        [TestMethod]
        public void WorkingDayStandardTests()
        {
            var commandLine = new[] { "-wFriday", "--work-day=Wednesday", "-vwMonday", "-w", "Tuesday", "Friday", "-vw", "Tuesday", "--work-day", "Thursday", "--", "-wSaturday", "--work-day=Sunday" };
            var result = GetOpt.Parse(commandLine, TestOptions.Standard);

            Assert.AreEqual(3, result.NonOptions.Count);
            Assert.AreEqual("Friday", result.NonOptions[0]);
            Assert.AreEqual("-wSaturday", result.NonOptions[1]);
            Assert.AreEqual("--work-day=Sunday", result.NonOptions[2]);

            var options = result.Options;
            Assert.AreEqual(8, result.Options.Count);

            Assert.AreSame(TestOptions.Standard.Single(o => o.ShortName == 'w'), options[0].Definition);
            Assert.AreEqual(WeekDay.Friday, options[0].Argument);

            Assert.AreSame(TestOptions.Standard.Single(o => o.ShortName == 'w'), options[1].Definition);
            Assert.AreEqual(WeekDay.Wednesday, options[1].Argument);

            Assert.AreSame(TestOptions.Standard.Single(o => o.ShortName == 'v'), options[2].Definition);
            Assert.IsNull(options[2].Argument);

            Assert.AreSame(TestOptions.Standard.Single(o => o.ShortName == 'w'), options[3].Definition);
            Assert.AreEqual(WeekDay.Monday, options[3].Argument);

            Assert.AreSame(TestOptions.Standard.Single(o => o.ShortName == 'w'), options[4].Definition);
            Assert.AreEqual(WeekDay.Tuesday, options[4].Argument);

            Assert.AreSame(TestOptions.Standard.Single(o => o.ShortName == 'v'), options[5].Definition);
            Assert.IsNull(options[5].Argument);

            Assert.AreSame(TestOptions.Standard.Single(o => o.ShortName == 'w'), options[6].Definition);
            Assert.AreEqual(WeekDay.Tuesday, options[6].Argument);

            Assert.AreSame(TestOptions.Standard.Single(o => o.ShortName == 'w'), options[6].Definition);
            Assert.AreEqual(WeekDay.Thursday, options[7].Argument);
        }

        [TestMethod]
        public void WorkingDayOutOfRange()
        {
            static void CheckForOutOfRange(IList<string> arguments)
            {
                Assert.ThrowsException<GetOptException>(() => GetOpt.Parse(arguments, TestOptions.Standard));
            }

            CheckForOutOfRange(new[] { "-wSaturday" });
            CheckForOutOfRange(new[] { "-w", "Sunday" });
            CheckForOutOfRange(new[] { "--work-day=Sunday" });
        }

        [TestMethod]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public void WorkingDayInvalid()
        {
            static void CheckForInvalidEnum(IList<string> arguments)
            {
                Assert.ThrowsException<GetOptException>(() => GetOpt.Parse(arguments, TestOptions.Standard));
            }

            CheckForInvalidEnum(new[] { "-wMOnday" });
            CheckForInvalidEnum(new[] { "-w", "tuesday" });
            CheckForInvalidEnum(new[] { "--work-day=bullshit" });
        }

        [TestMethod]
        public void WorkDayArgumentMissing()
        {
            static void CheckForMissingArgument(IList<string> arguments)
            {
                Assert.ThrowsException<GetOptException>(() => GetOpt.Parse(arguments, TestOptions.Standard));
            }

            CheckForMissingArgument(new[] { "-w" });
            CheckForMissingArgument(new[] { "--work-day" });
            CheckForMissingArgument(new[] { "--work-day", "--", "Monday" });
            CheckForMissingArgument(new[] { "-w", "--", "Tuesday" });
        }
    }
}
