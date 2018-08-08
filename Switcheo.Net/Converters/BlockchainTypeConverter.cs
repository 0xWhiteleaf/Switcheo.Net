using CryptoExchange.Net.Converters;
using Switcheo.Net.Objects;
using System.Collections.Generic;

namespace Switcheo.Net.Converters
{
    public class BlockchainTypeConverter : BaseConverter<BlockchainType>
    {
        public BlockchainTypeConverter() : this(true) { }
        public BlockchainTypeConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<BlockchainType, string> Mapping => new Dictionary<BlockchainType, string>()
        {
            { BlockchainType.Neo, "neo" },
            { BlockchainType.Qtum, "qtum" },
            { BlockchainType.Ethereum, "ethereum" }
        };
    }
}
