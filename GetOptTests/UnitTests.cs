using System.Linq;
using De.Hochstaetter.GetOpt;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace De.Hochstaetter.GetOptTests
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void WorkingDayStandardTests()
        {
            var commandLine = new[] { "-wFriday", "--work-day=Wednesday", "-vwMonday", "-w", "Tuesday", "Friday", "-vw", "Tuesday", "--", "-wSaturday", "--work-day=Sunday" };
            var result = commandLine.ArgumentList(TestOptions.Standard);

            Assert.AreEqual(3, result.NonOptions.Count);
            Assert.AreEqual("Friday",result.NonOptions[0]);
            Assert.AreEqual("-wSaturday",result.NonOptions[1]);
            Assert.AreEqual("--work-day=Sunday",result.NonOptions[2]);

            var options = result.Options;
            Assert.AreEqual(7,result.Options.Count);

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

        }
    }
}
