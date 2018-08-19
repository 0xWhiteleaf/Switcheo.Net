using Switcheo.Net.Attributes;

namespace Switcheo.Net.Objects
{
    /// <summary>
    /// The withdrawal status
    /// </summary>
    public enum WithdrawalStatus
    {
        Unknown,
        Pending,
        Confirming,
        Success,
        Expired
    }

    /// <summary>
    /// The fill status
    /// </summary>
    public enum FillStatus
    {
        Unknown,
        Pending,
        Confirming,
        Success,
        Expired
    }

    /// <summary>
    /// The make status
    /// </summary>
    public enum MakeStatus
    {
        Unknown,
        Pending,
        Confirming,
        Success,
        Cancelling,
        Cancelled,
        Expired
    }

    /// <summary>
    /// The order type
    /// </summary>
    public enum OrderType
    {
        Unknown,
        Limit,
    }

    /// <summary>
    /// The order status
    /// </summary>
    public enum OrderStatus
    {
        Unknown,
        Pending,
        Processed,
        Expired
    }

    /// <summary>
    /// The order side
    /// </summary>
    public enum OrderSide
    {
        Unknown,
        Buy,
        Sell
    }

    /// <summary>
    /// The balance type
    /// </summary>
    public enum BalanceType
    {
        Unknown,
        Confirming,
        Confirmed,
        Locked
    }

    /// <summary>
    /// The event type
    /// Actually used in <see cref="SwitcheoWithdrawalExecutedResult"/> and <see cref="SwitcheoAssetConfirming.SwitcheoConfirmingEvent"/>
    /// </summary>
    public enum EventType
    {
        Unknown,
        Deposit,
        Withdrawal
    }

    /// <summary>
    /// The type of operation
    /// Actually used in <see cref="SwitcheoScriptParams"/>
    /// </summary>
    public enum OperationType
    {
        Unknown,
        Deposit,
        CancelOffer
    }

    /// <summary>
    /// The interval between candlesticks
    /// </summary>
    public enum CandlestickInterval
    {
        Unknown,
        OneMinute = 1,
        FiveMinutes = 5,
        ThirtyMinutes = 30,
        OneHour = 60,
        SixHours = 360,
        OneDay = 1440
    }

    /// <summary>
    /// The restricted "bases" symbols imposed by Switcheo
    /// </summary>
    public enum BaseSymbol
    {
        Unknown,
        [Symbol("NEO")]
        Neo,
        [Symbol("GAS")]
        Gas,
        [Symbol("SWTH")]
        Swth,
        [Symbol("USD")]
        Usd
    }

    /// <summary>
    /// The type of blockchain
    /// </summary>
    public enum BlockchainType
    {
        Unknown,
        [Symbol("NEO")]
        Neo,
        [Symbol("QTUM")]
        Qtum,
        [Symbol("ETH")]
        Ethereum
    }
}
