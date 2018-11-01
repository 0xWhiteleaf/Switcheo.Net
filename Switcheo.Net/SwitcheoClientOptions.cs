using CryptoExchange.Net.Objects;
using Switcheo.Net.Objects;

namespace Switcheo.Net
{
    public class SwitcheoClientOptions : ExchangeOptions
    {
        private const string ProductionApiAddress = "https://api.switcheo.network";
        private const string TestApiAddress = "https://test-api.switcheo.network";

        public bool UseTestApi { get; private set; }

        /// <summary>
        /// Construct a new SwitcheoClientOptions instance
        /// </summary>
        /// <param name="useTestApi">Indicate to the client if he must use testnet address</param>
        public SwitcheoClientOptions(bool useTestApi = false)
        {
            this.UseTestApi = useTestApi;

            if (!this.UseTestApi)
                BaseAddress = ProductionApiAddress;
            else
                BaseAddress = TestApiAddress;
        }

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

        /// <summary>
        /// The default connection timeout for an RPC node
        /// </summary>
        public int RpcConnectionTimeout { get; set; } = 20000;
    }
}
