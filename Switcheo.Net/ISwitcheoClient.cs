using CryptoExchange.Net;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.RateLimiter;
using Switcheo.Net.Objects;
using System;
using System.Security;
using System.Threading.Tasks;

namespace Switcheo.Net
{
    public interface ISwitcheoClient
    {
        /// <summary>
        /// Set the private key and the key type
        /// </summary>
        /// <param name="privateKey">The private key</param>
        /// <param name="keyType">The blockchain where the key is from (e.g. Neo, Qtum, Ethereum)</param>
        void SetApiCredentials(SecureString privateKey, BlockchainType keyType);

        /// <summary>
        /// Set the default contract hash, it's used if no contract hash is passed in methods that require a contract
        /// This avoids having to re-specify the contract hash at each call
        /// </summary>
        /// <param name="contractHash"></param>
        void SetDefaultContract(string contractHash);

        /// <summary>
        /// Synchronized version of the <see cref="SwitcheoClient.GetServerTimeAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<DateTime> GetServerTime(bool resetAutoTimestamp = false);

        /// <summary>
        /// Requests the server for the local time. This function also determines the offset between server and local time and uses this for subsequent API calls
        /// </summary>
        /// <returns>Server time</returns>
        Task<CallResult<DateTime>> GetServerTimeAsync(bool resetAutoTimestamp = false);

        /// <summary>
        /// Synchronized version of the <see cref="SwitcheoClient.GetCandlesticksAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<SwitcheoCandlestick[]> GetCandlesticks(string pair, DateTime startTime, DateTime endTime, CandlestickInterval interval);

        /// <summary>
        /// Returns candlestick chart data filtered by parameters
        /// </summary>
        /// <param name="pair">The pair to get the candlestick for (e.g. SWTH_NEO)</param>
        /// <param name="startTime">Start of time range for data</param>
        /// <param name="endTime">End of time range for data</param>
        /// <param name="interval">Candlestick period in minutes (e.g. 1, 5, 30, 60, 360, 1440)</param>
        /// <returns></returns>
        Task<CallResult<SwitcheoCandlestick[]>> GetCandlesticksAsync(string pair, DateTime startTime, DateTime endTime, CandlestickInterval interval);

        /// <summary>
        /// Synchronized version of the <see cref="SwitcheoClient.Get24HPricesAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<Switcheo24hPrice[]> Get24HPrices();

        /// <summary>
        /// Returns 24-hour data for all pairs and markets
        /// </summary>
        /// <returns></returns>
        Task<CallResult<Switcheo24hPrice[]>> Get24HPricesAsync();

        /// <summary>
        /// Synchronized version of the <see cref="SwitcheoClient.GetLastPricesAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<SwitcheoPricesList> GetLastPrices(string[] symbols = null);

        /// <summary>
        /// Returns last price of given symbol(s)
        /// By defaults, return all symbols
        /// </summary>
        /// <param name="symbols">(Optional) Return the price for these symbols</param>
        /// <returns></returns>
        Task<CallResult<SwitcheoPricesList>> GetLastPricesAsync(string[] symbols = null);

        /// <summary>
        /// Synchronized version of the <see cref="SwitcheoClient.GetOffersAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<SwitcheoOffer[]> GetOffers(BlockchainType blockchainType, string pair, string contractHash = null);

        /// <summary>
        /// Retrieves the best 70 offers (per side) on the offer book
        /// </summary>
        /// <param name="blockchainType">Filter offers for a specific blockchain (e.g. Neo)</param>
        /// <param name="pair">Filter offers for a pair (e.g. SWTH_NEO)</param>
        /// <param name="contractHash">(Optional if you have set a DefaultContractHash) Filter offers for a specific contract (e.g. eed0d2e14b0027f5f30ade45f2b23dc57dd54ad2)</param>
        /// <returns></returns>
        Task<CallResult<SwitcheoOffer[]>> GetOffersAsync(BlockchainType blockchainType, string pair, string contractHash = null);

        /// <summary>
        /// Synchronized version of the <see cref="SwitcheoClient.GetTradesAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<SwitcheoTrade[]> GetTrades(string pair, int? limit = null, DateTime? from = null, DateTime? to = null,
            string contractHash = null);

        /// <summary>
        /// Retrieves trades that have already occurred on Switcheo Exchange filtered by parameters
        /// </summary>
        /// <param name="pair">Filter trades for a pair (e.g. SWTH_NEO)</param>
        /// <param name="limit">(Optional) Return this number of trades (min: 1, max: 10000, default: 5000)</param>
        /// <param name="from">(Optional) Filter trades after this time</param>
        /// <param name="to">(Optional) Filter trades before this time</param>
        /// <param name="contractHash">(Optional if you have set a DefaultContractHash) Filter trades for a specific contract (e.g. eed0d2e14b0027f5f30ade45f2b23dc57dd54ad2)</param>
        /// <returns></returns>
        Task<CallResult<SwitcheoTrade[]>> GetTradesAsync(string pair, int? limit = null, DateTime? from = null,
            DateTime? to = null, string contractHash = null);

        /// <summary>
        /// Synchronized version of the <see cref="SwitcheoClient.GetOrdersAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<SwitcheoOrder[]> GetOrders(string address, string pair = null, string contractHash = null);

        /// <summary>
        /// Retrieves orders from a specific address filtered by the given parameters
        /// </summary>
        /// <param name="address">Filter orders made by this address</param>
        /// <param name="pair">(Optional) Filter orders to this pair (e.g. SWTH_NEO)</param>
        /// <param name="contractHash">(Optional if you have set a DefaultContractHash) Filter orders for a specific contract (e.g. eed0d2e14b0027f5f30ade45f2b23dc57dd54ad2)</param>
        /// <returns></returns>
        Task<CallResult<SwitcheoOrder[]>> GetOrdersAsync(string address, string pair = null, string contractHash = null);

        /// <summary>
        /// Synchronized version of the <see cref="SwitcheoClient.GetBalancesAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<SwitcheoBalancesList> GetBalances(string address, string contractHash = null);

        /// <summary>
        /// List contract balances of the given address and contract
        /// </summary>
        /// <param name="address">Filter balances for this address</param>
        /// <param name="contractHash">(Optional if you have set a DefaultContractHash) Filter balances for a specific contract (e.g. eed0d2e14b0027f5f30ade45f2b23dc57dd54ad2)</param>
        /// <returns></returns>
        Task<CallResult<SwitcheoBalancesList>> GetBalancesAsync(string address, string contractHash = null);

        /// <summary>
        /// Synchronized version of the <see cref="SwitcheoClient.GetBalancesAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<SwitcheoBalancesList> GetBalances(string[] addresses, string[] contractHashes);

        /// <summary>
        /// List contract balances of the given addresses and contracts
        /// The purpose of this endpoint is to allow convenient querying of a user's balance across multiple blockchains, for example, if you want to retrieve a user's NEO and ethereum balances
        /// As such, when using this endpoint, balances for the specified addresses and contract hashes will be merged and summed
        /// </summary>
        /// <param name="addresses">Filter balances for these addresses</param>
        /// <param name="contractHashes">Filter balances from these contract hashes (e.g. eed0d2e14b0027f5f30ade45f2b23dc57dd54ad2)</param>
        /// <returns></returns>
        Task<CallResult<SwitcheoBalancesList>> GetBalancesAsync(string[] addresses, string[] contractHashes);

        /// <summary>
        /// Synchronized version of the <see cref="SwitcheoClient.GetPairsAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<string[]> GetPairs(BaseSymbol[] bases = null);

        /// <summary>
        /// Returns available pairs filtered by the bases parameter
        /// By defaults, return all pairs
        /// </summary>
        /// <param name="bases">(Optional) Provides pairs for these base symbols (e.g. Neo, Gas, Swth, Usd)</param>
        /// <returns></returns>
        Task<CallResult<string[]>> GetPairsAsync(BaseSymbol[] bases = null);

        /// <summary>
        /// Retrieve a token in last tokens list
        /// </summary>
        /// <param name="symbolOrAssetId">The token (i.e. asset) symbol or id</param>
        /// <returns></returns>
        SwitcheoToken GetToken(string symbolOrAssetId);

        /// <summary>
        /// Synchronized version of the <see cref="SwitcheoClient.GetTokensAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<SwitcheoTokensList> GetTokens(bool refreshTokensList = false);

        /// <summary>
        /// Returns tokens deployed by Switcheo
        /// </summary>
        /// <returns></returns>
        Task<CallResult<SwitcheoTokensList>> GetTokensAsync(bool refreshTokensList = false);

        /// <summary>
        /// Retrieve a contract in last contracts list
        /// </summary>
        /// <param name="blockchainType">The blockchain where the contract is hosted (e.g. Neo, Qtum, ...)</param>
        /// <param name="version">The desired version of this contract (e.g. v1, v1.5, v2, ...)</param>
        /// <returns></returns>
        SwitcheoContract GetContract(BlockchainType blockchainType, string version);

        /// <summary>
        /// Synchronized version of the <see cref="SwitcheoClient.GetContractsAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<SwitcheoContractsList> GetContracts(bool refreshContractsList = false);

        /// <summary>
        /// Returns contracts deployed by Switcheo
        /// </summary>
        /// <returns></returns>
        Task<CallResult<SwitcheoContractsList>> GetContractsAsync(bool refreshContractsList = false);

        /// <summary>
        /// Synchronized version of the <see cref="SwitcheoClient.GetMyOrdersAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<SwitcheoOrder[]> GetMyOrders(string pair = null, string contractHash = null);

        /// <summary>
        /// Retrieves your orders filtered by the given parameters
        /// </summary>
        /// <param name="pair">(Optional) Filter orders to this pair (e.g. SWTH_NEO)</param>
        /// <param name="contractHash">(Optional if you have set a DefaultContractHash) Filter orders for a specific contract (e.g. eed0d2e14b0027f5f30ade45f2b23dc57dd54ad2)</param>
        /// <returns></returns>
        Task<CallResult<SwitcheoOrder[]>> GetMyOrdersAsync(string pair = null, string contractHash = null);

        /// <summary>
        /// Synchronized version of the <see cref="SwitcheoClient.GetMyBalancesAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<SwitcheoBalancesList> GetMyBalances(string contractHash = null);

        /// <summary>
        /// List your contract balances of the given contract
        /// </summary>
        /// <param name="contractHash">(Optional if you have set a DefaultContractHash) Filter balances for a specific contract (e.g. eed0d2e14b0027f5f30ade45f2b23dc57dd54ad2)</param>
        /// <returns></returns>
        Task<CallResult<SwitcheoBalancesList>> GetMyBalancesAsync(string contractHash = null);

        /// <summary>
        /// Synchronized version of the <see cref="SwitcheoClient.GetMyBalancesAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<SwitcheoBalancesList> GetMyBalances(string[] contractHashes);

        /// <summary>
        /// List your contract balances of the given contracts
        /// </summary>
        /// <param name="contractHashes">Filter balances from these contract hashes (e.g. eed0d2e14b0027f5f30ade45f2b23dc57dd54ad2)</param>
        /// <returns></returns>
        Task<CallResult<SwitcheoBalancesList>> GetMyBalancesAsync(string[] contractHashes);

        /// <summary>
        /// Synchronized version of the <see cref="SwitcheoClient.CreateDepositAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<SwitcheoDepositCreationResult> CreateDeposit(BlockchainType blockchainType, string asset, decimal amount,
            string contractHash = null);

        /// <summary>
        /// Creates a deposit which can be executed through <see cref="SwitcheoClient.ExecuteDeposit"/> or <see cref="SwitcheoClient.ExecuteDepositAsync"/>
        /// To be able to make a deposit, sufficient funds are required in the depositing wallet
        /// </summary>
        /// <param name="blockchainType">Blockchain that the token to deposit is on (e.g. Neo, Qtum, ...)</param>
        /// <param name="asset">The asset symbol or id to deposit (e.g. Neo, Swth, ...)</param>
        /// <param name="amount">Amount of tokens to deposit (e.g 1, 5, 10 ...)</param>
        /// <param name="contractHash">(Optional if you have set a DefaultContractHash) Contract hash to execute the deposit on (e.g. eed0d2e14b0027f5f30ade45f2b23dc57dd54ad2)</param>
        /// <returns></returns>
        Task<CallResult<SwitcheoDepositCreationResult>> CreateDepositAsync(BlockchainType blockchainType, string asset,
            decimal amount, string contractHash = null);

        /// <summary>
        /// Synchronized version of the <see cref="SwitcheoClient.ExecuteDepositAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<SwitcheoBasicResult> ExecuteDeposit(Guid depositId, SwitcheoTransaction transaction);

        /// <summary>
        /// Execute a deposit previously generated by <see cref="SwitcheoClient.CreateDeposit"/> or <see cref="SwitcheoClient.CreateDepositAsync"/>
        /// </summary>
        /// <param name="depositId">Id of the deposit returned during creation</param>
        /// <param name="transaction">Transaction associated to the deposit, also returned during creation</param>
        /// <returns></returns>
        Task<CallResult<SwitcheoBasicResult>> ExecuteDepositAsync(Guid depositId, SwitcheoTransaction transaction);

        /// <summary>
        /// Synchronized version of the <see cref="SwitcheoClient.CreateWithdrawalAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<SwitcheoWithdrawalCreationResult> CreateWithdrawal(BlockchainType blockchainType, string asset, decimal amount,
            string contractHash = null);

        /// <summary>
        /// Creates a withdrawal which can be executed through <see cref="SwitcheoClient.ExecuteWithdrawal"/> or <see cref="SwitcheoClient.ExecuteWithdrawalAsync"/>
        /// To be able to make a withdrawal, sufficient funds are required in the contract balance
        /// </summary>
        /// <param name="blockchainType">Blockchain that the token to withdrawal is on (e.g. Neo, Qtum, ...)</param>
        /// <param name="asset">The asset symbol or id to withdrawal (e.g. Neo, Swth, ...)</param>
        /// <param name="amount">Amount of tokens to withdrawal</param>
        /// <param name="contractHash">(Optional if you have set a DefaultContractHash) Contract hash to execute the withdrawal on (e.g. eed0d2e14b0027f5f30ade45f2b23dc57dd54ad2)</param>
        /// <returns></returns>
        Task<CallResult<SwitcheoWithdrawalCreationResult>> CreateWithdrawalAsync(BlockchainType blockchainType, string asset,
            decimal amount, string contractHash = null);

        /// <summary>
        /// Synchronized version of the <see cref="SwitcheoClient.ExecuteWithdrawalAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<SwitcheoWithdrawalExecutedResult> ExecuteWithdrawal(Guid withdrawalId);

        /// <summary>
        /// Execute a withdrawal previously generated by <see cref="SwitcheoClient.CreateWithdrawal"/> or <see cref="SwitcheoClient.CreateWithdrawalAsync"/>
        /// </summary>
        /// <param name="withdrawalId">Id of the withdrawal returned during creation</param>
        /// <returns></returns>
        Task<CallResult<SwitcheoWithdrawalExecutedResult>> ExecuteWithdrawalAsync(Guid withdrawalId);

        /// <summary>
        /// Synchronized version of the <see cref="SwitcheoClient.CreateOrderAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<SwitcheoOrder> CreateOrder(string pair, BlockchainType blockchainType, OrderSide side, decimal price,
            decimal wantAmount, bool useNativeToken, OrderType orderType, string contractHash = null);

        /// <summary>
        /// Creates an order which can be executed through <see cref="SwitcheoClient.ExecuteOrder"/> or <see cref="SwitcheoClient.ExecuteOrderAsync"/>
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
        Task<CallResult<SwitcheoOrder>> CreateOrderAsync(string pair, BlockchainType blockchainType, OrderSide side, decimal price,
            decimal wantAmount, bool useNativeToken, OrderType orderType, string contractHash = null);

        /// <summary>
        /// Synchronized version of the <see cref="SwitcheoClient.ExecuteOrderAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<SwitcheoOrder> ExecuteOrder(SwitcheoOrder order);

        /// <summary>
        ///  Execute an order previously generated by <see cref="SwitcheoClient.CreateOrder"/> or <see cref="SwitcheoClient.CreateOrderAsync"/>
        /// </summary>
        /// <param name="order">Order to execute</param>
        /// <returns></returns>
        Task<CallResult<SwitcheoOrder>> ExecuteOrderAsync(SwitcheoOrder order);

        /// <summary>
        /// Synchronized version of the <see cref="SwitcheoClient.CreateCancellationAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<SwitcheoCancellationCreationResult> CreateCancellation(Guid orderId);

        /// <summary>
        /// Creates a cancellation which can be executed through <see cref="SwitcheoClient.ExecuteCancellation"/> or <see cref="SwitcheoClient.ExecuteCancellationAsync"/>
        /// Only orders with makes and with an AvailableAmount of more than 0 can be cancelled
        /// </summary>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <returns></returns>
        Task<CallResult<SwitcheoCancellationCreationResult>> CreateCancellationAsync(Guid orderId);

        /// <summary>
        /// Synchronized version of the <see cref="SwitcheoClient.ExecuteCancellationAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<SwitcheoOrder> ExecuteCancellation(Guid cancellationId, SwitcheoTransaction cancellationTransaction);

        /// <summary>
        ///  Execute a cancellation previously generated by <see cref="SwitcheoClient.CreateCancellation"/> or <see cref="SwitcheoClient.CreateCancellationAsync"/>
        /// </summary>
        /// <param name="cancellationId">The id of the cancellation to execute</param>
        /// <returns></returns>
        Task<CallResult<SwitcheoOrder>> ExecuteCancellationAsync(Guid cancellationId, SwitcheoTransaction cancellationTransaction);

        void AddRateLimiter(IRateLimiter limiter);
        void RemoveRateLimiters();
        void Dispose();
        IRequestFactory RequestFactory { get; set; }
    }
}
