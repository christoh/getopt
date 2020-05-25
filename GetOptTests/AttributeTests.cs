using System.Collections.Generic;
using De.Hochstaetter.CommandLine;
using De.Hochstaetter.CommandLine.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace De.Hochstaetter.GetOptTests
{
    [TestClass]
    public class AttributeTests
    {
        [GetOpt(LongName = "verbose", ShortName = 'v', HasArgument = false)]
        private readonly bool verbose = false;

        [GetOpt(LongName = "log-file", ShortName = 'l')]
        private static string LogFile { get; set; }

        [GetOpt(LongName = "notify-email", ShortName = 'n')]
        public ISet<string> EmailNotifications { get; } = new HashSet<string>(new[] { "old", "stuff", "will", "be", "deleted" });

        [TestMethod]
        public void TestAttributesSuccess()
        {
            var getOpt = new GetOpt(this);

            Assert.AreEqual(3, getOpt.OptionDefinitions.Count);

            getOpt.Parse
            (
                "-vlC:\\Temp\\Logfile.txt",
                "-nroot@localhost",
                "--notify-email=admin@example.net",
                "-n", "donald@duck.com"
            );

            Assert.IsTrue(verbose);
            Assert.AreEqual("C:\\Temp\\Logfile.txt", LogFile);
            Assert.AreEqual(3, EmailNotifications.Count);
            Assert.IsTrue(EmailNotifications.Contains("root@localhost"));
            Assert.IsTrue(EmailNotifications.Contains("admin@example.net"));
            Assert.IsTrue(EmailNotifications.Contains("donald@duck.com"));
        }
    }
}
