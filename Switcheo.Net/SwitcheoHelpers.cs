using System;
using System.Globalization;

namespace Switcheo.Net
{
    public static class SwitcheoHelpers
    {
        public const char PairSeparator = '_';
        public const double DefaultAssetPrecision = 8;

        public static string GetOfferAsset(this string pair)
        {
            if (!pair.Contains(PairSeparator.ToString()))
                throw new Exception($"{pair} is not a valid pair");

            return pair.Split(PairSeparator)[0];
        }

        public static string GetWantAsset(this string pair)
        {
            if (!pair.Contains(PairSeparator.ToString()))
                throw new Exception($"{pair} is not a valid pair");

            return pair.Split(PairSeparator)[1];
        }

        public static string ToAssetAmount(this decimal value, double? assetPrecision = DefaultAssetPrecision)
        {
            if (assetPrecision < 0)
                throw new Exception($"Warning: {assetPrecision} is not valid");

            var assetMultiplier = Math.Pow(10, assetPrecision.Value);
            return (value * (decimal)assetMultiplier).ToString("#.##", CultureInfo.InvariantCulture);
        }

        public static decimal FromAssetAmount(this string input, double? assetPrecision = DefaultAssetPrecision)
        {
            if (assetPrecision < 0)
                throw new Exception($"Warning: {assetPrecision} is not valid");

            if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal value))
            {
                var assetMultiplier = Math.Pow(10, assetPrecision.Value);
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

        public static string RemoveZeroX(this string value)
        {
            return value.Substring(2, value.Length - 2);
        }

        public static string AddZeroX(this string value)
        {
            return $"0x{value}";
        }
    }
}
