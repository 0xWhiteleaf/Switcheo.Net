using CryptoExchange.Net.Converters;
using Switcheo.Net.Objects;
using System.Collections.Generic;

namespace Switcheo.Net.Converters
{
    public class OrderSideConverter : BaseConverter<OrderSide>
    {
        public OrderSideConverter() : this(true) { }
        public OrderSideConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<OrderSide, string> Mapping => new Dictionary<OrderSide, string>()
        {
            { OrderSide.Buy, "buy" },
            { OrderSide.Sell, "sell" },
        };
    }
}
