using NUnit.Framework;

namespace Switcheo.Net.UnitTests
{
    [TestFixture()]
    public class SwitcheoHelpersTest
    {
        [TestCase]
        public void ToNeoAssetAmount_Should_ConvertToNeoAssetAmount()
        {
            decimal amount1 = 22;
            decimal amount2 = 47.4m;
            decimal amount3 = 54678.0645m;

            decimal negativeAmount1 = -11;

            var toNeoAssetAmount1 = SwitcheoHelpers.ToNeoAssetAmount(amount1);
            var toNeoAssetAmount2 = SwitcheoHelpers.ToNeoAssetAmount(amount2);
            var toNeoAssetAmount3 = SwitcheoHelpers.ToNeoAssetAmount(amount3);

            var toNegativeNeoAssetAmount1 = SwitcheoHelpers.ToNeoAssetAmount(negativeAmount1);

            Assert.AreEqual("2200000000", toNeoAssetAmount1);
            Assert.AreEqual("4740000000", toNeoAssetAmount2);
            Assert.AreEqual("5467806450000", toNeoAssetAmount3);

            Assert.AreEqual("-1100000000", toNegativeNeoAssetAmount1);
        }

        [TestCase]
        public void FromNeoAssetAmount_Should_ConvertFromNeoAssetAmount()
        {
            string amount1 = "2200000000.0";
            string amount2 = "4740000000";
            string amount3 = "5467806450000";

            string negativeAmount1 = "-1100000000";

            var fromNeoAssetAmount1 = SwitcheoHelpers.FromNeoAssetAmount(amount1);
            var fromNeoAssetAmount2 = SwitcheoHelpers.FromNeoAssetAmount(amount2);
            var fromNeoAssetAmount3 = SwitcheoHelpers.FromNeoAssetAmount(amount3);

            var fromNegativeNeoAssetAmount1 = SwitcheoHelpers.FromNeoAssetAmount(negativeAmount1);

            Assert.AreEqual(22, fromNeoAssetAmount1);
            Assert.AreEqual(47.4m, fromNeoAssetAmount2);
            Assert.AreEqual(54678.0645m, fromNeoAssetAmount3);

            Assert.AreEqual(-11, fromNegativeNeoAssetAmount1);
        }
    }
}
