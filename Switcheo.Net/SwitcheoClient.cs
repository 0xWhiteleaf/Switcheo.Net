using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Switcheo.Net.Helpers;
using Switcheo.Net.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Switcheo.Net
{
    /// <summary>
    /// Client providing access to the Switcheo REST Api
    /// </summary>
    public class SwitcheoClient : ExchangeClient, ISwitcheoClient
    {
        #region Fields

        private JsonSerializer customSerializer;

        private static SwitcheoClientOptions defaultOptions = new SwitcheoClientOptions();
        
        private readonly object locker = new object();

        private bool autoTimestamp;
        private double timeOffset;
        private bool timeSynced;

        private string defaultContractHash;

        private SwitcheoContractsList lastContractsList;
        private SwitcheoTokensList lastTokensList;

        // Addresses
        private string baseApiAddress;

        // Versions
        private const string CurrentVersion = "2";

        // Methods
        private const string GetMethod = "GET";
        private const string PostMethod = "POST";
        private const string DeleteMethod = "DELETE";
        private const string PutMethod = "PUT";

        // Public
        private const string CheckTimeEndpoint = "exchange/timestamp";
        private const string CandlesticksEndpoint = "tickers/candlesticks";
        private const string Last24HoursEndpoint = "tickers/last_24_hours";
        private const string LastPriceEndpoint = "tickers/last_price";
        private const string ListOffersEndpoint = "offers";
        private const string ListTradesEndpoint = "trades";
        private const string ListBalancesEndpoint = "balances";
        private const string ListCurrencyPairsEndpoint = "exchange/pairs";
        private const string ListTokensEndpoint = "exchange/tokens";
        private const string ContractsListEndpoint = "exchange/contracts";

        // Signed
        private const string DepositsEndpoint = "deposits";
        private const string WithdrawalsEndpoint = "withdrawals";
        private const string OrdersEndpoint = "orders";
        private const string CancellationsEndpoint = "cancellations";

        // Actions
        private const string BroadcastAction = "broadcast";

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new instance of SwitcheoClient using the default options
        /// </summary>
        public SwitcheoClient() : this(defaultOptions) { }

        // <summary>
        /// Create a new instance of SwitcheoClient using provided options
        /// </summary>
        /// <param name="options">The options to use for this client</param>
        public SwitcheoClient(SwitcheoClientOptions options) : base(options, options.ApiCredentials == null ? null : new SwitcheoAuthenticationProvider(options.ApiCredentials, options.KeyType))
        {
            Configure(options);
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="options"></param>
        public static void SetDefaultOptions(SwitcheoClientOptions options)
        {
            defaultOptions = options;
        }

        /// <summary>
        /// Set the private key and the key type
        /// </summary>
        /// <param name="privateKey">The private key</param>
        /// <param name="keyType">The blockchain where the key is from (e.g. Neo, Qtum, Ethereum)</param>
        public void SetApiCredentials(PrivateKey privateKey, BlockchainType keyType)
        {
            SetAuthenticationProvider(new SwitcheoAuthenticationProvider(new ApiCredentials(privateKey), keyType));
        }

        /// <summary>
        /// Set the default contract hash, it's used if no contract hash is passed in methods that require a contract
        /// This avoids having to re-specify the contract hash at each call
        /// </summary>
        /// <param name="contractHash"></param>
        public void SetDefaultContract(string contractHash)
        {
            defaultContractHash = contractHash;
            log.Write(LogVerbosity.Info, $"You're now using {defaultContractHash} as default contract");
        }

        /// <summary>
        /// Synchronized version of the <see cref="GetServerTimeAsync(bool)"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<DateTime> GetServerTime(bool resetAutoTimestamp = false) => GetServerTimeAsync(resetAutoTimestamp).Result;

        /// <summary>
        /// Requests the server for the local time. This function also determines the offset between server and local time and uses this for subsequent API calls
        /// </summary>
        /// <returns>Server time</returns>
        public async Task<CallResult<DateTime>> GetServerTimeAsync(bool resetAutoTimestamp = false)
        {
            var url = GetUrl(CheckTimeEndpoint, CurrentVersion);
            if (!autoTimestamp)
            {
                var result = await ExecuteRequest<SwitcheoCheckTime>(url).ConfigureAwait(false);
                return new CallResult<DateTime>(result.Data?.ServerTime ?? default(DateTime), result.Error);
            }
            else
            {
                var localTime = DateTime.UtcNow;
                var sw = Stopwatch.StartNew();
                var result = await ExecuteRequest<SwitcheoCheckTime>(url).ConfigureAwait(false);
                if (!result.Success)
                    return new CallResult<DateTime>(default(DateTime), result.Error);

                if (!timeSynced || resetAutoTimestamp)
                {
                    // Calculate time offset between local and server by taking the elapsed time request time / 2 (round trip)
                    timeOffset = ((result.Data.ServerTime - localTime).TotalMilliseconds) - sw.ElapsedMilliseconds / 2.0;
                    timeSynced = true;
                    log.Write(LogVerbosity.Info, $"Time offset set to {timeOffset}ms");
                }
                return new CallResult<DateTime>(result.Data.ServerTime, result.Error);
            }
        }

        /// <summary>
        /// Synchronized version of the <see cref="GetCandlesticksAsync(string, DateTime, DateTime, CandlestickInterval)"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<SwitcheoCandlestick[]> GetCandlesticks(string pair, DateTime startTime, DateTime endTime, CandlestickInterval interval)
            => GetCandlesticksAsync(pair, startTime, endTime, interval).Result;

        /// <summary>
        /// Returns candlestick chart data filtered by parameters
        /// </summary>
        /// <param name="pair">The pair to get the candlestick for (e.g. SWTH_NEO)</param>
        /// <param name="startTime">Start of time range for data</param>
        /// <param name="endTime">End of time range for data</param>
        /// <param name="interval">Candlestick period in minutes (e.g. 1, 5, 30, 60, 360, 1440)</param>
        /// <returns></returns>
        public async Task<CallResult<SwitcheoCandlestick[]>> GetCandlesticksAsync(string pair, DateTime startTime, DateTime endTime, CandlestickInterval interval)
        {
            var parameters = new Dictionary<string, object>() { { "pair", pair } };
            parameters.AddParameter("start_time", ToUnixTimestamp(startTime));
            parameters.AddParameter("end_time", ToUnixTimestamp(endTime));
            parameters.AddParameter("interval", (int)interval);
            return await ExecuteRequest<SwitcheoCandlestick[]>(GetUrl(CandlesticksEndpoint, CurrentVersion), GetMethod, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="Get24HPricesAsync"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<Switcheo24hPrice[]> Get24HPrices() => Get24HPricesAsync().Result;

        /// <summary>
        /// Returns 24-hour data for all pairs and markets
        /// </summary>
        /// <returns></returns>
        public async Task<CallResult<Switcheo24hPrice[]>> Get24HPricesAsync()
        {
            return await ExecuteRequest<Switcheo24hPrice[]>(GetUrl(Last24HoursEndpoint, CurrentVersion)).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="GetLastPricesAsync(string[])"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<SwitcheoPricesList> GetLastPrices(string[] symbols = null) => GetLastPricesAsync(symbols).Result;

        /// <summary>
        /// Returns last price of given symbol(s)
        /// By defaults, return all symbols
        /// </summary>
        /// <param name="symbols">(Optional) Return the price for these symbols</param>
        /// <returns></returns>
        public async Task<CallResult<SwitcheoPricesList>> GetLastPricesAsync(string[] symbols = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("symbols", symbols);
            return await ExecuteRequest<SwitcheoPricesList>(GetUrl(LastPriceEndpoint, CurrentVersion), GetMethod, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="GetOffersAsync(BlockchainType, string, string)"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<SwitcheoOffer[]> GetOffers(BlockchainType blockchainType, string pair, string contractHash = null)
            => GetOffersAsync(blockchainType, pair, contractHash).Result;

        /// <summary>
        /// Retrieves the best 70 offers (per side) on the offer book
        /// </summary>
        /// <param name="blockchainType">Filter offers for a specific blockchain (e.g. Neo)</param>
        /// <param name="pair">Filter offers for a pair (e.g. SWTH_NEO)</param>
        /// <param name="contractHash">(Optional if you have set a DefaultContractHash) Filter offers for a specific contract (e.g. eed0d2e14b0027f5f30ade45f2b23dc57dd54ad2)</param>
        /// <returns></returns>
        public async Task<CallResult<SwitcheoOffer[]>> GetOffersAsync(BlockchainType blockchainType, string pair, string contractHash = null)
        {
            contractHash = this.GetContractHash(contractHash);

            var parameters = new Dictionary<string, object>() { { "blockchain", blockchainType.ToString().ToLower() } };
            parameters.AddParameter("pair", pair);
            parameters.AddParameter("contract_hash", contractHash);
            return await ExecuteRequest<SwitcheoOffer[]>(GetUrl(ListOffersEndpoint, CurrentVersion), GetMethod, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="GetTradesAsync(string, int?, DateTime?, DateTime?, string)"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<SwitcheoTrade[]> GetTrades(string pair, int? limit = null, DateTime? from = null, DateTime? to = null, string contractHash = null)
            => GetTradesAsync(pair, limit, from, to, contractHash).Result;

        /// <summary>
        /// Retrieves trades that have already occurred on Switcheo Exchange filtered by parameters
        /// </summary>
        /// <param name="pair">Filter trades for a pair (e.g. SWTH_NEO)</param>
        /// <param name="limit">(Optional) Return this number of trades (min: 1, max: 10000, default: 5000)</param>
        /// <param name="from">(Optional) Filter trades after this time</param>
        /// <param name="to">(Optional) Filter trades before this time</param>
        /// <param name="contractHash">(Optional if you have set a DefaultContractHash) Filter trades for a specific contract (e.g. eed0d2e14b0027f5f30ade45f2b23dc57dd54ad2)</param>
        /// <returns></returns>
        public async Task<CallResult<SwitcheoTrade[]>> GetTradesAsync(string pair, int? limit = null, DateTime? from = null, DateTime? to = null, string contractHash = null)
        {
            contractHash = this.GetContractHash(contractHash);

            var parameters = new Dictionary<string, object>() { { "contract_hash", contractHash } };
            parameters.AddParameter("pair", pair);
            parameters.AddOptionalParameter("from", from != null ? ToUnixTimestamp(from.Value).ToString() : null);
            parameters.AddOptionalParameter("to", to != null ? ToUnixTimestamp(to.Value).ToString() : null);
            parameters.AddOptionalParameter("limit", limit?.ToString());
            return await ExecuteRequest<SwitcheoTrade[]>(GetUrl(ListTradesEndpoint, CurrentVersion), GetMethod, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="GetOrdersAsync(string, string, string)"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<SwitcheoOrder[]> GetOrders(string address, string pair = null, string contractHash = null)
            => GetOrdersAsync(address, pair, contractHash).Result;

        /// <summary>
        /// Retrieves orders from a specific address filtered by the given parameters
        /// </summary>
        /// <param name="address">Filter orders made by this address</param>
        /// <param name="pair">(Optional) Filter orders to this pair (e.g. SWTH_NEO)</param>
        /// <param name="contractHash">(Optional if you have set a DefaultContractHash) Filter orders for a specific contract (e.g. eed0d2e14b0027f5f30ade45f2b23dc57dd54ad2)</param>
        /// <returns></returns>
        public async Task<CallResult<SwitcheoOrder[]>> GetOrdersAsync(string address, string pair = null, string contractHash = null)
        {
            contractHash = this.GetContractHash(contractHash);

            var parameters = new Dictionary<string, object>() { { "address", address } };
            parameters.AddParameter("contract_hash", contractHash);
            parameters.AddOptionalParameter("pair", pair);
            return await ExecuteRequest<SwitcheoOrder[]>(GetUrl(OrdersEndpoint, CurrentVersion), GetMethod, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="GetBalancesAsync(string, string)"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<SwitcheoBalancesList> GetBalances(string address, string contractHash = null)
            => GetBalancesAsync(address, contractHash).Result;

        /// <summary>
        /// List contract balances of the given address and contract
        /// </summary>
        /// <param name="address">Filter balances for this address</param>
        /// <param name="contractHash">(Optional if you have set a DefaultContractHash) Filter balances for a specific contract (e.g. eed0d2e14b0027f5f30ade45f2b23dc57dd54ad2)</param>
        /// <returns></returns>
        public async Task<CallResult<SwitcheoBalancesList>> GetBalancesAsync(string address, string contractHash = null)
        {
            contractHash = this.GetContractHash(contractHash);

            return await GetBalancesAsync(new string[] { address }, new string[] { contractHash });
        }

        /// <summary>
        /// Synchronized version of the <see cref="GetBalancesAsync(string[], string[])"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<SwitcheoBalancesList> GetBalances(string[] addresses, string[] contractHashes)
            => GetBalancesAsync(addresses, contractHashes).Result;

        /// <summary>
        /// List contract balances of the given addresses and contracts
        /// The purpose of this endpoint is to allow convenient querying of a user's balance across multiple blockchains, for example, if you want to retrieve a user's NEO and ethereum balances
        /// As such, when using this endpoint, balances for the specified addresses and contract hashes will be merged and summed
        /// </summary>
        /// <param name="addresses">Filter balances for these addresses</param>
        /// <param name="contractHashes">Filter balances from these contract hashes (e.g. eed0d2e14b0027f5f30ade45f2b23dc57dd54ad2)</param>
        /// <returns></returns>
        public async Task<CallResult<SwitcheoBalancesList>> GetBalancesAsync(string[] addresses, string[] contractHashes)
        {
            var parameters = new Dictionary<string, object>() { { "addresses", addresses } };
            parameters.AddParameter("contract_hashes", contractHashes);
            return await ExecuteRequest<SwitcheoBalancesList>(GetUrl(ListBalancesEndpoint, CurrentVersion), GetMethod, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="GetPairsAsync(BaseSymbol[])"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<string[]> GetPairs(BaseSymbol[] bases = null) => GetPairsAsync(bases).Result;

        /// <summary>
        /// Returns available pairs filtered by the bases parameter
        /// By defaults, return all pairs
        /// </summary>
        /// <param name="bases">(Optional) Provides pairs for these base symbols (e.g. Neo, Gas, Swth, Usd)</param>
        /// <returns></returns>
        public async Task<CallResult<string[]>> GetPairsAsync(BaseSymbol[] bases = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("bases", bases != null ? bases.Select(s => s.GetSymbol()).ToArray() : null);
            return await ExecuteRequest<string[]>(GetUrl(ListCurrencyPairsEndpoint, CurrentVersion), GetMethod, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve a token in last tokens list
        /// </summary>
        /// <param name="symbolOrAssetId">The token (i.e. asset) symbol or id</param>
        /// <returns></returns>
        public SwitcheoToken GetToken(string symbolOrAssetId)
        {
            SwitcheoToken token = null;

            if (this.lastTokensList == null)
                this.GetTokens(true);

            token = this.lastTokensList?.Tokens?.FirstOrDefault(t => t.Symbol.ToLower() == symbolOrAssetId.ToLower() || t.Id.ToLower() == symbolOrAssetId.ToLower());

            return token;
        }

        /// <summary>
        /// Synchronized version of the <see cref="GetTokensAsync"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<SwitcheoTokensList> GetTokens(bool refreshTokensList = false) => GetTokensAsync(refreshTokensList).Result;

        /// <summary>
        /// Returns updated hashes of contracts deployed by Switcheo
        /// </summary>
        /// <returns></returns>
        public async Task<CallResult<SwitcheoTokensList>> GetTokensAsync(bool refreshTokensList = false)
        {
            var tokensList = await ExecuteRequest<SwitcheoTokensList>(GetUrl(ListTokensEndpoint, CurrentVersion)).ConfigureAwait(false);
            if (tokensList.Success && refreshTokensList)
                lastTokensList = tokensList.Data;

            return tokensList;
        }

        /// <summary>
        /// Retrieve a contract in last contracts list
        /// </summary>
        /// <param name="blockchainType">The blockchain where the contract is hosted (e.g. Neo, Qtum, ...)</param>
        /// <param name="version">The desired version of this contract (e.g. v1, v1.5, v2, ...)</param>
        /// <returns></returns>
        public SwitcheoContract GetContract(BlockchainType blockchainType, string version)
        {
            SwitcheoContract contract = null;

            if (this.lastContractsList == null)
                this.GetContracts(true);

            if (version.Contains('.'))
                version = version.Replace('.', '_');

            version = version.ToUpper();

            switch (blockchainType)
            {
                case BlockchainType.Neo:
                    contract = this.lastContractsList.NeoContracts.FirstOrDefault(c => c.Version == version);
                    break;

                case BlockchainType.Qtum:
                    contract = this.lastContractsList.QtumContracts.FirstOrDefault(c => c.Version == version);
                    break;

                case BlockchainType.Ethereum:
                    contract = this.lastContractsList.EthereumContracts.FirstOrDefault(c => c.Version == version);
                    break;
            }

            if (contract == null)
                throw new Exception($"Unable to resolve contract : {blockchainType} ({version})");

            return contract;
        }

        /// <summary>
        /// Synchronized version of the <see cref="GetContracts(bool)"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<SwitcheoContractsList> GetContracts(bool refreshContractsList = false) => GetContractsAsync(refreshContractsList).Result;

        /// <summary>
        /// Returns contracts deployed by Switcheo
        /// </summary>
        /// <returns></returns>
        public async Task<CallResult<SwitcheoContractsList>> GetContractsAsync(bool refreshContractsList = false)
        {
            var contractsListResult = await ExecuteRequest<SwitcheoContractsList>(GetUrl(ContractsListEndpoint, CurrentVersion)).ConfigureAwait(false);
            if (contractsListResult.Success && refreshContractsList)
                this.lastContractsList = contractsListResult.Data;

            return contractsListResult;
        }

        #endregion

        #region Signed

        /// <summary>
        /// Synchronized version of the <see cref="GetMyOrdersAsync(string, string)"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<SwitcheoOrder[]> GetMyOrders(string pair = null, string contractHash = null)
            => GetMyOrdersAsync(pair, contractHash).Result;

        /// <summary>
        /// Retrieves your orders filtered by the given parameters
        /// </summary>
        /// <param name="pair">(Optional) Filter orders to this pair (e.g. SWTH_NEO)</param>
        /// <param name="contractHash">(Optional if you have set a DefaultContractHash) Filter orders for a specific contract (e.g. eed0d2e14b0027f5f30ade45f2b23dc57dd54ad2)</param>
        /// <returns></returns>
        public async Task<CallResult<SwitcheoOrder[]>> GetMyOrdersAsync(string pair = null, string contractHash = null)
        {
            this.CheckSignatureAbilities();

            contractHash = this.GetContractHash(contractHash);

            return await GetOrdersAsync(GetAuthProvider().Address, pair, contractHash).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="GetMyBalancesAsync(string)"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<SwitcheoBalancesList> GetMyBalances(string contractHash = null) => GetMyBalancesAsync(contractHash).Result;

        /// <summary>
        /// List your contract balances of the given contract
        /// </summary>
        /// <param name="contractHash">(Optional if you have set a DefaultContractHash) Filter balances for a specific contract (e.g. eed0d2e14b0027f5f30ade45f2b23dc57dd54ad2)</param>
        /// <returns></returns>
        public async Task<CallResult<SwitcheoBalancesList>> GetMyBalancesAsync(string contractHash = null)
        {
            this.CheckSignatureAbilities();

            contractHash = this.GetContractHash(contractHash);

            return await GetMyBalancesAsync(new string[] { contractHash }).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="GetMyBalancesAsync(string[])"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<SwitcheoBalancesList> GetMyBalances(string[] contractHashes) => GetMyBalancesAsync(contractHashes).Result;

        /// <summary>
        /// List your contract balances of the given contracts
        /// </summary>
        /// <param name="contractHashes">Filter balances from these contract hashes (e.g. eed0d2e14b0027f5f30ade45f2b23dc57dd54ad2)</param>
        /// <returns></returns>
        public async Task<CallResult<SwitcheoBalancesList>> GetMyBalancesAsync(string[] contractHashes)
        {
            this.CheckSignatureAbilities();

            var neoAddress = this.GetAuthProvider().Address;
            //TODO: MergeUserAddresses (planned when Switcheo release Ethereum and Qtum)

            return await GetBalancesAsync(new string[] { neoAddress }, contractHashes).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="CreateDepositAsync(BlockchainType, string, decimal, string)"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<SwitcheoDepositCreationResult> CreateDeposit(BlockchainType blockchainType, string asset, decimal amount, string contractHash = null)
            => CreateDepositAsync(blockchainType, asset, amount, contractHash).Result;

        /// <summary>
        /// Creates a deposit which can be executed through <see cref="ExecuteDeposit(Guid, SwitcheoTransaction)"/> or <see cref="ExecuteDepositAsync(Guid, SwitcheoTransaction)"/>
        /// To be able to make a deposit, sufficient funds are required in the depositing wallet
        /// </summary>
        /// <param name="blockchainType">Blockchain that the token to deposit is on (e.g. Neo, Qtum, ...)</param>
        /// <param name="asset">The asset symbol or id to deposit (e.g. Neo, Swth, ...)</param>
        /// <param name="amount">Amount of tokens to deposit (e.g 1, 5, 10 ...)</param>
        /// <param name="contractHash">(Optional if you have set a DefaultContractHash) Contract hash to execute the deposit on (e.g. eed0d2e14b0027f5f30ade45f2b23dc57dd54ad2)</param>
        /// <returns></returns>
        public async Task<CallResult<SwitcheoDepositCreationResult>> CreateDepositAsync(BlockchainType blockchainType, string asset, decimal amount, string contractHash = null)
        {
            this.CheckSignatureAbilities();
            await CheckAutoTimestamp().ConfigureAwait(false);

            contractHash = this.GetContractHash(contractHash);

            var token = this.GetToken(asset);
            if (token == null)
                throw new Exception($"Your asset {asset} is not listed yet on Switcheo. If your asset is listed since few minutes only, try to call GetTokens(refreshTokensList: true) in order to refresh tokens list.");

            var parameters = new Dictionary<string, object>() { { "blockchain", blockchainType.ToString().ToLower() } };
            parameters.AddParameter("asset_id", asset);
            parameters.AddParameter("amount", amount.ToAssetAmount(token.Precision));
            parameters.AddParameter("contract_hash", contractHash);
            parameters.AddParameter("address", GetAuthProvider().Address);
            parameters.AddParameter("timestamp", GetTimestamp());
            return await ExecuteRequest<SwitcheoDepositCreationResult>(GetUrl(DepositsEndpoint, CurrentVersion), PostMethod, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="ExecuteDepositAsync(Guid, SwitcheoTransaction)"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<SwitcheoBasicResult> ExecuteDeposit(Guid depositId, SwitcheoTransaction transaction) => ExecuteDepositAsync(depositId, transaction).Result;

        /// <summary>
        /// Execute a deposit previously generated by <see cref="CreateDeposit(BlockchainType, string, decimal, string)"/> or <see cref="CreateDepositAsync(BlockchainType, string, decimal, string)"/>
        /// </summary>
        /// <param name="depositId">Id of the deposit returned during creation</param>
        /// <param name="transaction">Transaction associated to the deposit, also returned during creation</param>
        /// <returns></returns>
        public async Task<CallResult<SwitcheoBasicResult>> ExecuteDepositAsync(Guid depositId, SwitcheoTransaction transaction)
        {
            this.CheckSignatureAbilities();

            var signedTransaction = transaction.ToSignedTransaction();

            var serializedTransaction = signedTransaction.Serialize(false);
            var signature = this.GetAuthProvider().Sign(serializedTransaction).ToHexString();

            var parameters = new Dictionary<string, object>() { { "signature", signature } };
            return await ExecuteRequest<SwitcheoBasicResult>(GetUrl($"{DepositsEndpoint}/{depositId}/{BroadcastAction}", CurrentVersion), PostMethod, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="CreateWithdrawalAsync(BlockchainType, string, decimal, string)"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<SwitcheoWithdrawalCreationResult> CreateWithdrawal(BlockchainType blockchainType, string asset, decimal amount, string contractHash = null)
            => CreateWithdrawalAsync(blockchainType, asset, amount, contractHash).Result;

        /// <summary>
        /// Creates a withdrawal which can be executed through <see cref="ExecuteWithdrawal(Guid)"/> or <see cref="ExecuteWithdrawalAsync(Guid)"/>
        /// To be able to make a withdrawal, sufficient funds are required in the contract balance
        /// </summary>
        /// <param name="blockchainType">Blockchain that the token to withdrawal is on (e.g. Neo, Qtum, ...)</param>
        /// <param name="assetId">The asset symbol or id to withdrawal (e.g. Neo, Swth, ...)</param>
        /// <param name="amount">Amount of tokens to withdrawal</param>
        /// <param name="contractHash">(Optional if you have set a DefaultContractHash) Contract hash to execute the withdrawal on (e.g. eed0d2e14b0027f5f30ade45f2b23dc57dd54ad2)</param>
        /// <returns></returns>
        public async Task<CallResult<SwitcheoWithdrawalCreationResult>> CreateWithdrawalAsync(BlockchainType blockchainType, string asset, decimal amount, string contractHash = null)
        {
            this.CheckSignatureAbilities();
            await CheckAutoTimestamp().ConfigureAwait(false);

            contractHash = this.GetContractHash(contractHash);

            var token = this.GetToken(asset);
            if (token == null)
                throw new Exception($"Your asset {asset} is not listed yet on Switcheo. If your asset is listed since few minutes only, try to call GetTokens(refreshTokensList: true) in order to refresh tokens list.");

            var parameters = new Dictionary<string, object>() { { "blockchain", blockchainType.ToString().ToLower() } };
            parameters.AddParameter("asset_id", asset);
            parameters.AddParameter("amount", amount.ToAssetAmount(token.Precision));
            parameters.AddParameter("contract_hash", contractHash);
            parameters.AddParameter("address", GetAuthProvider().Address);
            parameters.AddParameter("timestamp", GetTimestamp());
            return await ExecuteRequest<SwitcheoWithdrawalCreationResult>(GetUrl(WithdrawalsEndpoint, CurrentVersion), PostMethod, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="ExecuteWithdrawalAsync(Guid)"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<SwitcheoWithdrawalExecutedResult> ExecuteWithdrawal(Guid withdrawalId) => ExecuteWithdrawalAsync(withdrawalId).Result;

        /// <summary>
        /// Execute a withdrawal previously generated by <see cref="CreateWithdrawal(BlockchainType, string, decimal, string)"/> or <see cref="CreateWithdrawalAsync(BlockchainType, string, decimal, string)"/>
        /// </summary>
        /// <param name="withdrawalId">Id of the withdrawal returned during creation</param>
        /// <returns></returns>
        public async Task<CallResult<SwitcheoWithdrawalExecutedResult>> ExecuteWithdrawalAsync(Guid withdrawalId)
        {
            this.CheckSignatureAbilities();
            await CheckAutoTimestamp().ConfigureAwait(false);

            var parameters = new Dictionary<string, object>() { { "id", withdrawalId.ToString() } };
            parameters.AddParameter("timestamp", GetTimestamp());
            return await ExecuteRequest<SwitcheoWithdrawalExecutedResult>(GetUrl($"{WithdrawalsEndpoint}/{withdrawalId}/{BroadcastAction}", CurrentVersion), PostMethod, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="CreateOrderAsync(string, BlockchainType, OrderSide, decimal, decimal, bool, OrderType, string)"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<SwitcheoOrder> CreateOrder(string pair, BlockchainType blockchainType, OrderSide side,
            decimal price, decimal wantAmount, bool useNativeToken, OrderType orderType, string contractHash = null)
            => CreateOrderAsync(pair, blockchainType, side, price, wantAmount, useNativeToken, orderType, contractHash).Result;

        /// <summary>
        /// Creates an order which can be executed through <see cref="ExecuteOrder(SwitcheoOrder)"/> or <see cref="ExecuteOrderAsync(SwitcheoOrder)"/>
        /// Orders can only be created after sufficient funds have been deposited into the user's contract balance
        /// A successful order will have zero or one make and/or zero or more fills
        /// </summary>
        /// <param name="pair">Pair to trade (e.g. RPX_NEO)</param>
        /// <param name="blockchainType">Blockchain that the pair is on (e.g. Neo, Qtum, ...)</param>
        /// <param name="side">Whether to buy or sell on this pair</param>
        /// <param name="price">Buy or sell price</param>
        /// <param name="wantAmount">Amount of tokens offered in the order</param>
        /// <param name="useNativeToken">Whether to use SWTH as fees or not</param>
        /// <param name="orderType">The order type (e.g. Limit, ...)</param>
        /// <param name="contractHash">(Optional if you have set a DefaultContractHash) Contract hash to execute the order on (e.g. eed0d2e14b0027f5f30ade45f2b23dc57dd54ad2)</param>
        /// <returns></returns>
        public async Task<CallResult<SwitcheoOrder>> CreateOrderAsync(string pair, BlockchainType blockchainType, OrderSide side,
            decimal price, decimal wantAmount, bool useNativeToken, OrderType orderType, string contractHash = null)
        {
            this.CheckSignatureAbilities();
            await CheckAutoTimestamp().ConfigureAwait(false);

            contractHash = this.GetContractHash(contractHash);

            string wantAsset = SwitcheoHelpers.GetWantAsset(pair);
            var token = this.GetToken(wantAsset);
            if (token == null)
                throw new Exception($"Your want asset {wantAsset} is not listed yet on Switcheo. If your asset is listed since few minutes only, try to call GetTokens(refreshTokensList: true) in order to refresh tokens list.");

            var parameters = new Dictionary<string, object>() { { "pair", pair } };
            parameters.AddParameter("blockchain", blockchainType.ToString().ToLower());
            parameters.AddParameter("side", side.ToString().ToLower());
            parameters.AddParameter("price", price.ToFixedEightDecimals());
            parameters.AddParameter("want_amount", wantAmount.ToAssetAmount(token.Precision));
            parameters.AddParameter("use_native_tokens", useNativeToken);
            parameters.AddParameter("order_type", orderType.ToString().ToLower());
            parameters.AddParameter("timestamp", GetTimestamp());
            parameters.AddParameter("address", GetAuthProvider().Address);
            parameters.AddParameter("contract_hash", contractHash);
            return await ExecuteRequest<SwitcheoOrder>(GetUrl(OrdersEndpoint, CurrentVersion), PostMethod, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="ExecuteOrderAsync(SwitcheoOrder)"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<SwitcheoOrder> ExecuteOrder(SwitcheoOrder order)
            => ExecuteOrderAsync(order).Result;

        /// <summary>
        ///  Execute an order previously generated by <see cref="CreateOrder(string, BlockchainType, OrderSide, decimal, decimal, bool, OrderType, string)"/> or <see cref="CreateOrderAsync(string, BlockchainType, OrderSide, decimal, decimal, bool, OrderType, string)"/>
        /// </summary>
        /// <param name="order">Order to execute</param>
        /// <returns></returns>
        public async Task<CallResult<SwitcheoOrder>> ExecuteOrderAsync(SwitcheoOrder order)
        {
            this.CheckSignatureAbilities();

            var parameters = new Dictionary<string, object>() { { "signatures", order.GetSignatures(GetAuthProvider()) } };
            return await ExecuteRequest<SwitcheoOrder>(GetUrl($"{OrdersEndpoint}/{order.Id}/{BroadcastAction}", CurrentVersion), PostMethod, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="CreateCancellationAsync(Guid)"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<SwitcheoCancellationCreationResult> CreateCancellation(Guid orderId)
            => CreateCancellationAsync(orderId).Result;

        /// <summary>
        /// Creates a cancellation which can be executed through <see cref="ExecuteCancellation(Guid, SwitcheoTransaction)"/> or <see cref="ExecuteCancellationAsync(Guid, SwitcheoTransaction)"/>
        /// Only orders with makes and with an AvailableAmount of more than 0 can be cancelled
        /// </summary>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <returns></returns>
        public async Task<CallResult<SwitcheoCancellationCreationResult>> CreateCancellationAsync(Guid orderId)
        {
            this.CheckSignatureAbilities();
            await CheckAutoTimestamp().ConfigureAwait(false);

            var parameters = new Dictionary<string, object>() { { "order_id", orderId.ToString() } };
            parameters.AddParameter("timestamp", GetTimestamp());
            parameters.AddParameter("address", GetAuthProvider().Address);
            return await ExecuteRequest<SwitcheoCancellationCreationResult>(GetUrl(CancellationsEndpoint, CurrentVersion), PostMethod, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="ExecuteCancellationAsync(Guid, SwitcheoTransaction)"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<SwitcheoOrder> ExecuteCancellation(Guid cancellationId, SwitcheoTransaction cancellationTransaction)
            => ExecuteCancellationAsync(cancellationId, cancellationTransaction).Result;

        /// <summary>
        ///  Execute a cancellation previously generated by <see cref="CreateCancellation(Guid)"/> or <see cref="CreateCancellationAsync(Guid)"/>
        /// </summary>
        /// <param name="cancellationId">The id of the cancellation to execute</param>
        /// <returns></returns>
        public async Task<CallResult<SwitcheoOrder>> ExecuteCancellationAsync(Guid cancellationId, SwitcheoTransaction cancellationTransaction)
        {
            this.CheckSignatureAbilities();

            var signedTransaction = cancellationTransaction.ToSignedTransaction();

            var serializedTransaction = signedTransaction.Serialize(false);
            var signature = this.GetAuthProvider().Sign(serializedTransaction).ToHexString();

            var parameters = new Dictionary<string, object>() { { "signature", signature } };
            return await ExecuteRequest<SwitcheoOrder>(GetUrl($"{CancellationsEndpoint}/{cancellationId}/{BroadcastAction}", CurrentVersion), PostMethod, parameters, true).ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region Helpers

        private void Configure(SwitcheoClientOptions options)
        {
            base.Configure(options);
            if (options.ApiCredentials != null)
                SetAuthenticationProvider(new SwitcheoAuthenticationProvider(options.ApiCredentials, options.KeyType));

            baseApiAddress = options.BaseAddress;
            defaultContractHash = options.DefaultContractHash;
            autoTimestamp = options.AutoTimestamp;

            customSerializer = JsonSerializer.Create(new JsonSerializerSettings()
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                Context = new StreamingContext(StreamingContextStates.Other, this)
            });
        }

        private Uri GetUrl(string endpoint, string version)
        {
            var result = $"{baseApiAddress}/v{version}/{endpoint}";
            return new Uri(result);
        }

        private long ToUnixTimestamp(DateTime time, bool millis = false)
        {
            long unixTimeStamp = -1;

            var timeSpan = (time - new DateTime(1970, 1, 1));
            if (!millis)
                unixTimeStamp = (long)timeSpan.TotalSeconds;
            else
                unixTimeStamp = (long)timeSpan.TotalMilliseconds;

            return unixTimeStamp;
        }

        private string GetTimestamp()
        {
            var offset = autoTimestamp ? timeOffset : 0;
            return ToUnixTimestamp(DateTime.UtcNow.AddMilliseconds(offset), true).ToString();
        }

        private async Task CheckAutoTimestamp()
        {
            if (autoTimestamp && !timeSynced)
                await GetServerTimeAsync().ConfigureAwait(false);
        }

        private string GetContractHash(string contractHash)
        {
            string _contractHash = null;

            if (string.IsNullOrEmpty(contractHash))
            {
                if (!string.IsNullOrEmpty(this.defaultContractHash))
                {
                    _contractHash = this.defaultContractHash;
                }
                else
                {
                    throw new Exception("You don't have specified any default contract hash, in this case you need to pass a contract hash in parameters to this method");
                }
            }
            else
            {
                _contractHash = contractHash;
            }

            return _contractHash;
        }
        
        private void CheckSignatureAbilities()
        {
            if (this.GetAuthProvider() == null || !this.GetAuthProvider().CanSign)
                throw new Exception("You must provide a valid private key to use signed endpoints");
        }

        private SwitcheoAuthenticationProvider GetAuthProvider()
        {
            return (SwitcheoAuthenticationProvider)base.authProvider;
        }

        private async Task CheckTokensList()
        {
            if (lastTokensList == null || lastTokensList.Tokens == null || lastTokensList.Tokens.Count() == 0)
                await this.GetTokensAsync(true).ConfigureAwait(false);
        }

        #endregion

        #region Overrides

        protected override IRequest ConstructRequest(Uri uri, string method, Dictionary<string, object> parameters, bool signed)
        {
            var request = base.ConstructRequest(uri, method, parameters, signed);

            if (signed && parameters != null && parameters.Count > 0)
            {
                request.ContentType = "application/json";
                request.Accept = "application/json";

                switch (this.GetAuthProvider().KeyType)
                {
                    case BlockchainType.Neo:
                        JObject requestObject = new JObject();

                        var signatureParam = parameters.FirstOrDefault(x => x.Key == "signature");
                        if (signatureParam.IsDefault())
                        {
                            // If request is not already signed...
                            var sortedPrameters = parameters.OrderBy(x => x.Key);
                            foreach (var param in sortedPrameters)
                            {
                                if (param.Key != "address")
                                    requestObject[param.Key] = JToken.FromObject(param.Value);
                            }

                            string parameterString = requestObject.ToString(Formatting.None);
                            string parameterHexString = Encoding.UTF8.GetBytes(parameterString).ToHexString();

                            string lengthHex = (parameterHexString.Length / 2).ToString("X").PadLeft(2, '0');
                            string concatenatedString = lengthHex + parameterHexString;

                            string serializedTransaction = SwitcheoAuthenticationProvider.ledgerCompatiblePrefix +
                                concatenatedString + SwitcheoAuthenticationProvider.ledgerCompatibleSuffix;

                            
                            requestObject["signature"] = this.GetAuthProvider().Sign(serializedTransaction.HexToBytes()).ToHexString();

                            var address = parameters.FirstOrDefault(x => x.Key == "address");
                            if (!address.IsDefault())
                                requestObject[address.Key] = JToken.FromObject(address.Value);
                        }
                        else
                        {
                            requestObject["signature"] = signatureParam.Value.ToString();
                        }

                        lock (locker)
                        {
                            byte[] _requestObject = Encoding.UTF8.GetBytes(requestObject.ToString(Formatting.None));
                            using (var stream = request.GetRequestStream().Result)
                            {
                                stream.Write(_requestObject, 0, _requestObject.Length);
                            }
                        }
                        break;

                    case BlockchainType.Qtum:
                        throw new NotImplementedException();

                    case BlockchainType.Ethereum:
                        throw new NotImplementedException();
                }
            }

            return request;
        }

        protected override async Task<CallResult<T>> ExecuteRequest<T>(Uri uri, string method = "GET", Dictionary<string, object> parameters = null, bool signed = false)
        {
            log.Write(LogVerbosity.Debug, $"Creating request for " + uri);
            if (signed && authProvider == null)
            {
                log.Write(LogVerbosity.Warning, $"Request {uri.AbsolutePath} failed because no ApiCredentials were provided");
                return new CallResult<T>(null, new NoApiCredentialsError());
            }

            var request = ConstructRequest(uri, method, parameters, signed);

            if (apiProxy != null)
            {
                log.Write(LogVerbosity.Debug, "Setting proxy");
                request.SetProxy(apiProxy.Host, apiProxy.Port);
            }

            string paramString = null;
            if (parameters != null)
            {
                paramString = "with parameters";

                foreach (var param in parameters)
                    paramString += $" {param.Key}={(param.Value.GetType().IsArray ? $"[{string.Join(", ", ((object[])param.Value).Select(p => p.ToString()))}]" : param.Value)},";

                paramString = paramString.Trim(',');
            }

            log.Write(LogVerbosity.Debug, $"Sending {(signed ? "signed" : "")} request to {request.Uri} {(paramString ?? "")}");
            var result = await ExecuteRequest(request).ConfigureAwait(false);
            return result.Error != null ? new CallResult<T>(null, result.Error) : Deserialize<T>(result.Data, false, this.customSerializer);
        }

        private async Task<CallResult<string>> ExecuteRequest(IRequest request)
        {
            var returnedData = "";
            try
            {
                var response = await request.GetResponse().ConfigureAwait(false);
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    returnedData = await reader.ReadToEndAsync().ConfigureAwait(false);
                    log.Write(LogVerbosity.Debug, "Data returned: " + returnedData);
                    return new CallResult<string>(returnedData, null);
                }
            }
            catch (WebException we)
            {
                var response = (HttpWebResponse)we.Response;
                try
                {
                    var reader = new StreamReader(response.GetResponseStream());
                    var responseData = await reader.ReadToEndAsync().ConfigureAwait(false);
                    log.Write(LogVerbosity.Warning, "Server returned an error: " + responseData);
                    return new CallResult<string>(null, ParseErrorResponse(responseData));
                }
                catch (Exception)
                {
                }

                var infoMessage = "No response from server";
                if (response == null)
                {
                    infoMessage += $" | {we.Status} - {we.Message}";
                    log.Write(LogVerbosity.Warning, infoMessage);
                    return new CallResult<string>(null, new WebError(infoMessage));
                }

                infoMessage = $"Status: {response.StatusCode}-{response.StatusDescription}, Message: {we.Message}";
                log.Write(LogVerbosity.Warning, infoMessage);
                return new CallResult<string>(null, new ServerError(infoMessage));
            }
            catch (Exception e)
            {
                log.Write(LogVerbosity.Error, $"Unkown error occured: {e.GetType()}, {e.Message}, {e.StackTrace}");
                return new CallResult<string>(null, new UnknownError(e.Message + ", data: " + returnedData));
            }
        }

        protected override Error ParseErrorResponse(string error)
        {
            if (error == null)
                return new ServerError("Unknown error, no error message");

            var obj = JObject.Parse(error);

            if (obj.ContainsKey("error"))
                return new ServerError((string)obj["error"]);

            if (obj.ContainsKey("errors"))
                return new ServerError((string)obj["errors"]);

            return new ServerError(error);
        }

        #endregion
    }
}
