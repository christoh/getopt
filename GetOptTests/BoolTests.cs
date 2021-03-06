﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using De.Hochstaetter.CommandLine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace De.Hochstaetter.GetOptTests
{
    [TestClass]
    public class BoolTests
    {
        [TestMethod]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public void BoolTestsStandard()
        {
            var commandLine = new[]
            {
                "-e1", "-efalse", "-e", "yes", "--show-minor-errors=no", "dummy", "--show-minor-errors=true", "-e", "0",
                "--", "-etrue", "--show-minor-errors",
            };

            var result = GetOpt.Parse(commandLine, TestOptions.Standard);
            var (nonOptions, options) = (result.NonOptions, result.Options);

            Assert.AreEqual(3, nonOptions.Count);
            Assert.AreEqual("dummy", nonOptions[0]);
            Assert.AreEqual("-etrue", nonOptions[1]);
            Assert.AreEqual("--show-minor-errors", nonOptions[2]);

            var definition = TestOptions.Standard.Single(o => o.ShortName == 'e');

            Assert.AreEqual(6, options.Count);
            Assert.IsTrue(options.All(o => ReferenceEquals(definition, o.Definition)));
            Assert.IsTrue(options[0].Argument);
            Assert.IsFalse(options[1].Argument);
            Assert.IsTrue(options[2].Argument);
            Assert.IsFalse(options[3].Argument);
            Assert.IsTrue(options[4].Argument);
            Assert.IsFalse(options[5].Argument);
        }

        [TestMethod]
        public void BoolWrongArguments()
        {
            void CheckWrongBoolArguments(IList<string> arguments)
            {
                Assert.ThrowsException<GetOptException>(() => GetOpt.Parse(arguments, TestOptions.Standard));
            }

            CheckWrongBoolArguments(new[] { "-eTrue" });
            CheckWrongBoolArguments(new[] { "-e", "False" });
            CheckWrongBoolArguments(new[] { "--show-minor-errors=dummy" });
        }
    }
}
