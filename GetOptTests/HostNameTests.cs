using System.Linq;
using De.Hochstaetter.CommandLine;
using De.Hochstaetter.CommandLine.Exceptions;
using De.Hochstaetter.CommandLine.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace De.Hochstaetter.GetOptTests
{
    [TestClass]
    public class HostNameTests
    {
        [TestMethod]
        public void HostNameRegexPassTests()
        {
            var result = GetOpt.Parse
            (
                TestOptions.Standard,
                Parameters.Default,
                "-Hwww.google.com", "--host-name=1api.net", "--host-name", "ftp.microsoft.com"
            );

            Assert.AreEqual(0, result.NonOptions.Count);
            var options = result.Options;
            Assert.AreEqual(3, options.Count);

            var hostNameOption = TestOptions.Standard.Single(o => o.ShortName == 'H');

            Assert.AreEqual(hostNameOption, options[0].Definition);
            Assert.AreEqual("www.google.com", options[0].Argument);

            Assert.AreEqual(hostNameOption, options[1].Definition);
            Assert.AreEqual("1api.net", options[1].Argument);

            Assert.AreEqual(hostNameOption, options[2].Definition);
            Assert.AreEqual("ftp.microsoft.com", options[2].Argument);
        }

        [TestMethod]
        public void HostNameRegexFailTests()
        {
            static void CheckHostName(params string[] arguments)
            {
                Assert.ThrowsException<GetOptException>(() => GetOpt.Parse(arguments, TestOptions.Standard));
            }

            CheckHostName("-H_sip._udp.phone-company.com");
            CheckHostName("--host-name=thisHostnameIsExtraordinaryLongAndThusFailsTheRegexCheckAccordingToRfc1123");
            CheckHostName("--host-name", "*.google.com");
        }
    }
}
