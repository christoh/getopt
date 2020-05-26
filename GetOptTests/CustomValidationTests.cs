using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.RegularExpressions;
using De.Hochstaetter.CommandLine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace De.Hochstaetter.GetOptTests
{
    [TestClass]
    public class CustomValidationTests
    {
        private readonly IEnumerable<OptionDefinition> options = new[]
        {
            new OptionDefinition("remote-host", 'r', typeof(string), validator:IsValidIpAddressOrHostName),
        };

        [TestMethod]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public void TestCustomValidationSuccess()
        {
            GetOpt.Parse
            (
                options, null,
                "-r127.0.0.1",
                "-r127.0.0.256", // this is a valid hostname according to RFC 1123 (pray that your gethostbyname knows that)
                "--remote-host=www.xn--hochsttter-v5a.de",
                "-r", "2001:db8::dead:beef:FACE:B00C",
                "--remote-host", "::FFFF:192.168.0.1"
            );
        }

        [TestMethod]
        public void TestCustomValidationFail()
        {
            var getOpt = new GetOpt(options, Parameters.Default);

            void CheckRemoteHost(params string[] arguments)
            {
                Assert.ThrowsException<GetOptException>(() => getOpt.Parse(arguments));
            }

            CheckRemoteHost("-r127.0.0.1/32");
            CheckRemoteHost("-r", "2001:db8::dead:beef:FACE:B00K");
            CheckRemoteHost("--remote-host", "_sip._udp.dus.net");
            CheckRemoteHost("--remote-host=www.hochstätter.de");
        }

        private static bool IsValidIpAddressOrHostName(string stringArgument, dynamic argument)
        {
            return
                Regex.IsMatch
                (
                    stringArgument,
                    @"^([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])(\.([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]{0,61}[a-zA-Z0-9]))*$"
                )

                ||

                IPAddress.TryParse(stringArgument, out _);
        }
    }
}
