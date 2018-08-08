using CryptoExchange.Net.Converters;
using Switcheo.Net.Objects;
using System.Collections.Generic;

namespace Switcheo.Net.Converters
{
    public class OrderStatusConverter : BaseConverter<OrderStatus>
    {
        public OrderStatusConverter() : this(true) { }
        public OrderStatusConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<OrderStatus, string> Mapping => new Dictionary<OrderStatus, string>()
        {
            { OrderStatus.Pending, "pending" },
            { OrderStatus.Processed, "processed" },
            { OrderStatus.Expired, "expired" }
        };
    }
}
