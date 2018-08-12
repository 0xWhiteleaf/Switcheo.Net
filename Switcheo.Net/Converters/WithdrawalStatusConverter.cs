using CryptoExchange.Net.Converters;
using Switcheo.Net.Objects;
using System.Collections.Generic;

namespace Switcheo.Net.Converters
{
    public class WithdrawalStatusConverter : BaseConverter<WithdrawalStatus>
    {
        public WithdrawalStatusConverter() : this(true) { }
        public WithdrawalStatusConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<WithdrawalStatus, string> Mapping => new Dictionary<WithdrawalStatus, string>()
        {
            { WithdrawalStatus.Pending, "pending" },
            { WithdrawalStatus.Confirming, "confirming" },
            { WithdrawalStatus.Success, "success" },
            { WithdrawalStatus.Expired, "expired" },
        };
    }
}