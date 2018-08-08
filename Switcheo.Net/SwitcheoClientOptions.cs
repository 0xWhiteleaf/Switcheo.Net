using CryptoExchange.Net;
using Switcheo.Net.Objects;

namespace Switcheo.Net
{
    public class SwitcheoClientOptions : ExchangeOptions
    {
        private const string ProductionApiAddress = "https://api.switcheo.network";
        private const string TestApiAddress = "https://test-api.switcheo.network";

        private string _baseAddress = null;

        /// <summary>
        /// The base address used to connect to the API
        /// </summary>
        public string BaseAddress
        {
            get
            {
                if (_baseAddress == null)
                {
                    if (!this.UseTestApi)
                        _baseAddress = ProductionApiAddress;
                    else
                        _baseAddress = TestApiAddress;
                }

                return _baseAddress;
            }
            set
            {
                if (!this.UseTestApi)
                    _baseAddress = value;
            }
        }

        /// <summary>
        /// Indicate to the client if he must use testnet address
        /// </summary>
        public bool UseTestApi { get; set; } = false;

        /// <summary>
        /// Whether or not to automatically sync the local time with the server time
        /// </summary>
        public bool AutoTimestamp { get; set; } = false;

        /// <summary>
        /// The blockchain where the key is from (e.g. Neo, Qtum, Ethereum)
        /// </summary>
        public BlockchainType KeyType { get; set; } = BlockchainType.Neo;

        /// <summary>
        /// The default contract hash, used if no contract hash is passed in methods that require a contract
        /// This avoids having to re-specify the contract hash at each call
        /// </summary>
        public string DefaultContractHash { get; set; }
    }
}
