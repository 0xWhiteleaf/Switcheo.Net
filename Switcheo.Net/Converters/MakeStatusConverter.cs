using CryptoExchange.Net.Converters;
using Switcheo.Net.Objects;
using System.Collections.Generic;

namespace Switcheo.Net.Converters
{
    public class MakeStatusConverter : BaseConverter<MakeStatus>
    {
        public MakeStatusConverter() : this(true) { }
        public MakeStatusConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<MakeStatus, string> Mapping => new Dictionary<MakeStatus, string>()
        {
            { MakeStatus.Pending, "pending" },
            { MakeStatus.Confirming, "confirming" },
            { MakeStatus.Success, "success" },
            { MakeStatus.Cancelling, "cancelling" },
            { MakeStatus.Cancelled, "cancelled" },
            { MakeStatus.Expired, "expired" }
        };
    }
}
