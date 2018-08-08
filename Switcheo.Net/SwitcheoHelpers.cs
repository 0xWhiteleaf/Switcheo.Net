using System;
using System.Globalization;

namespace Switcheo.Net
{
    public static class SwitcheoHelpers
    {
        public const char PairSeparator = '_';
        public const double NeoAssetPrecision = 8;

        public static string GetFromAsset(this string pair)
        {
            if (!pair.Contains(PairSeparator.ToString()))
                throw new Exception($"{pair} is not a valid pair");

            return pair.Split(PairSeparator)[0];
        }

        public static string GetToAsset(this string pair)
        {
            if (!pair.Contains(PairSeparator.ToString()))
                throw new Exception($"{pair} is not a valid pair");

            return pair.Split(PairSeparator)[1];
        }

        public static string ToNeoAssetAmount(this decimal value)
        {
            var assetMultiplier = Math.Pow(10, NeoAssetPrecision);
            return (value * (decimal)assetMultiplier).ToString("#.##", CultureInfo.InvariantCulture);
        }

        public static decimal FromNeoAssetAmount(this string input)
        {
            if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal value))
            {
                var assetMultiplier = Math.Pow(10, NeoAssetPrecision);
                return value / (decimal)assetMultiplier;
            }
            else
            {
                return 0m;
            }
        }

        public static string ToFixedEightDecimals(this decimal value)
        {
            return value.ToString("N8", CultureInfo.InvariantCulture);
        }
    }
}
