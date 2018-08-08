using CryptoExchange.Net.Converters;
using Switcheo.Net.Objects;
using System.Collections.Generic;

namespace Switcheo.Net.Converters
{
    public class OperationTypeConverter : BaseConverter<OperationType>
    {
        public OperationTypeConverter() : this(true) { }
        public OperationTypeConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<OperationType, string> Mapping => new Dictionary<OperationType, string>()
        {
            { OperationType.Deposit, "deposit" },
            { OperationType.CancelOffer, "cancelOffer" }
        };
    }
}
