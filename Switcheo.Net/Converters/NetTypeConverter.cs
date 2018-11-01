using CryptoExchange.Net.Converters;
using Switcheo.Net.Objects;
using System.Collections.Generic;

namespace Switcheo.Net.Converters
{
    public class NetTypeConverter : BaseConverter<NetType>
    {
        public NetTypeConverter() : this(true) { }
        public NetTypeConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<NetType, string> Mapping => new Dictionary<NetType, string>()
        {
            { NetType.TestNet, "TestNet" },
            { NetType.MainNet, "MainNet" }
        };
    }
}
