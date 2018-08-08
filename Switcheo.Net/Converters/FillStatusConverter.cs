using CryptoExchange.Net.Converters;
using Switcheo.Net.Objects;
using System.Collections.Generic;

namespace Switcheo.Net.Converters
{
    public class FillStatusConverter : BaseConverter<FillStatus>
    {
        public FillStatusConverter() : this(true) { }
        public FillStatusConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<FillStatus, string> Mapping => new Dictionary<FillStatus, string>()
        {
            { FillStatus.Pending, "pending" },
            { FillStatus.Confirming, "confirming" },
            { FillStatus.Success, "success" },
            { FillStatus.Expired, "expired" },
        };
    }
}
