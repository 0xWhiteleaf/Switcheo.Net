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
    /// Asset that is actually supported by Switcheo API
    /// </summary>
    public enum SupportedAsset
    {
        Unknown,
        [Symbol("NEO")]
        [Asset("c56f33fc6ecfcd0c225c4ab356fee59390af8560be0e930faebe74a6daff7c9b")]
        Neo,
        [Symbol("GAS")]
        [Asset("602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7")]
        Gas,
        [Symbol("SWTH")]
        [Asset("ab38352559b8b203bde5fddfa0b07d8b2525e132")]
        Swth,
        [Symbol("ACAT")]
        [Asset("7f86d61ff377f1b12e589a5907152b57e2ad9a7a")]
        Acat,
        [Symbol("APH")]
        [Asset("a0777c3ce2b169d4a23bcba4565e3225a0122d95")]
        Aph,
        [Symbol("AVA")]
        [Asset("de2ed49b691e76754c20fe619d891b78ef58e537")]
        Ava,
        [Symbol("CPX")]
        [Asset("45d493a6f73fa5f404244a5fb8472fc014ca5885")]
        Cpx,
        [Symbol("DBC")]
        [Asset("b951ecbbc5fe37a9c280a76cb0ce0014827294cf")]
        Dbc,
        [Symbol("EFX")]
        [Asset("acbc532904b6b51b5ea6d19b803d78af70e7e6f9")]
        Efx,
        [Symbol("GALA")]
        [Asset("9577c3f972d769220d69d1c4ddbd617c44d067aa")]
        Gala,
        [Symbol("LRN")]
        [Asset("06fa8be9b6609d963e8fc63977b9f8dc5f10895f")]
        Lrn,
        [Symbol("MCT")]
        [Asset("a87cc2a513f5d8b4a42432343687c2127c60bc3f")]
        Mct,
        [Symbol("NKN")]
        [Asset("c36aee199dbba6c3f439983657558cfb67629599")]
        Nkn,
        [Symbol("OBT")]
        [Asset("0e86a40588f715fcaf7acd1812d50af478e6e917")]
        Obt,
        [Symbol("ONT")]
        [Asset("ceab719b8baa2310f232ee0d277c061704541cfb")]
        Ont,
        [Symbol("PKC")]
        [Asset("af7c7328eee5a275a3bcaee2bf0cf662b5e739be")]
        Pkc,
        [Symbol("RHT")]
        [Asset("2328008e6f6c7bd157a342e789389eb034d9cbc4")]
        Rht,
        [Symbol("RPX")]
        [Asset("ecc6b20d3ccac1ee9ef109af5a7cdb85706b1df9")]
        Rpx,
        [Symbol("TKY")]
        [Asset("132947096727c84c7f9e076c90f08fec3bc17f18")]
        Tky,
        [Symbol("TNC")]
        [Asset("08e8c4400f1af2c20c28e0018f29535eb85d15b6")]
        Tnc,
        [Symbol("TOLL")]
        [Asset("78fd589f7894bf9642b4a573ec0e6957dfd84c48")]
        Toll,
        [Symbol("QLC")]
        [Asset("0d821bd7b6d53f5c2b40e217c6defc8bbe896cf5")]
        Qlc,
        [Symbol("SOUL")]
        [Asset("ed07cffad18f1308db51920d99a2af60ac66a7b3")]
        Soul,
        [Symbol("ZPT")]
        [Asset("ac116d4b8d4ca55e6b6d4ecce2192039b51cccc5")]
        Zpt,
        [Symbol("SWH")]
        [Asset("78e6d16b914fe15bc16150aeb11d0c2a8e532bdd")]
        Swh
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
