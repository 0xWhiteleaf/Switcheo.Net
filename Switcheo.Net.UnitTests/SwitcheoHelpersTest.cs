using NUnit.Framework;
using System.Security;
using Switcheo.Net.Helpers;
using CryptoExchange.Net;
using NeoModules.Core;

namespace Switcheo.Net.UnitTests
{
    [TestFixture()]
    public class SwitcheoHelpersTest
    {
        [TestCase]
        public void ToAssetAmount_Should_ConvertToAssetAmount()
        {
            decimal amount1 = 22;
            decimal amount2 = 47.4m;
            decimal amount3 = 54678.0645m;
            decimal amount4 = 68;
            decimal amount5 = 127.15487m;

            decimal negativeAmount1 = -11;

            var toAssetAmount1 = SwitcheoHelpers.ToAssetAmount(amount1, SwitcheoClientTests.SampleTokensList[0].Precision);
            var toAssetAmount2 = SwitcheoHelpers.ToAssetAmount(amount2, SwitcheoClientTests.SampleTokensList[0].Precision);
            var toAssetAmount3 = SwitcheoHelpers.ToAssetAmount(amount3, SwitcheoClientTests.SampleTokensList[0].Precision);
            var toAssetAmount4 = SwitcheoHelpers.ToAssetAmount(amount4, SwitcheoClientTests.SampleTokensList[3].Precision);
            var toAssetAmount5 = SwitcheoHelpers.ToAssetAmount(amount5, SwitcheoClientTests.SampleTokensList[4].Precision);

            var toNegativeNeoAssetAmount1 = SwitcheoHelpers.ToAssetAmount(negativeAmount1, SwitcheoClientTests.SampleTokensList[0].Precision);

            Assert.AreEqual("2200000000", toAssetAmount1);
            Assert.AreEqual("4740000000", toAssetAmount2);
            Assert.AreEqual("5467806450000", toAssetAmount3);
            Assert.AreEqual("68", toAssetAmount4);
            Assert.AreEqual("127154870000000000000", toAssetAmount5);


            Assert.AreEqual("-1100000000", toNegativeNeoAssetAmount1);
        }

        [TestCase]
        public void FromAssetAmount_Should_ConvertFromAssetAmount()
        {
            string amount1 = "2200000000.0";
            string amount2 = "4740000000";
            string amount3 = "5467806450000";
            string amount4 = "68";
            string amount5 = "127154870000000000000";

            string negativeAmount1 = "-1100000000";

            var fromAssetAmount1 = SwitcheoHelpers.FromAssetAmount(amount1, SwitcheoClientTests.SampleTokensList[0].Precision);
            var fromAssetAmount2 = SwitcheoHelpers.FromAssetAmount(amount2, SwitcheoClientTests.SampleTokensList[0].Precision);
            var fromAssetAmount3 = SwitcheoHelpers.FromAssetAmount(amount3, SwitcheoClientTests.SampleTokensList[0].Precision);
            var fromAssetAmount4 = SwitcheoHelpers.FromAssetAmount(amount4, SwitcheoClientTests.SampleTokensList[3].Precision);
            var fromAssetAmount5 = SwitcheoHelpers.FromAssetAmount(amount5, SwitcheoClientTests.SampleTokensList[4].Precision);

            var fromNegativeNeoAssetAmount1 = SwitcheoHelpers.FromAssetAmount(negativeAmount1, SwitcheoClientTests.SampleTokensList[0].Precision);

            Assert.AreEqual(22, fromAssetAmount1);
            Assert.AreEqual(47.4m, fromAssetAmount2);
            Assert.AreEqual(54678.0645m, fromAssetAmount3);
            Assert.AreEqual(68, fromAssetAmount4);
            Assert.AreEqual(127.15487m, fromAssetAmount5);

            Assert.AreEqual(-11, fromNegativeNeoAssetAmount1);
        }

        [TestCase]
        public void IsNep2_Should_DetectEncryptedKeys()
        {
            SecureString correctNep2Wallet = "6PYWLPXtLHZeqR1vpWtcfSt5Ubwba3Qv1zdeacd7347mBfXABa9WXbe3Yh".ToSecureString();
            SecureString incorrectNep2Wallet = "KxmqXqdavQhziXNGborgiqy3JWjCmDHgJpKniMdFdnLHCBmCXnoU".ToSecureString();

            Assert.IsTrue(WalletsHelper.IsNep2(correctNep2Wallet));
            Assert.IsFalse(WalletsHelper.IsNep2(incorrectNep2Wallet));
        }
    }
}
