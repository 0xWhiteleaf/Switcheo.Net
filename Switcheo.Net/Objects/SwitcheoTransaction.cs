using CryptoExchange.Net.Attributes;
using NeoModules.Core;
using NeoModules.NEP6.Transactions;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Switcheo.Net.Objects
{
    /// <summary>
    /// Information about a specific transaction
    /// </summary>
    public class SwitcheoTransaction
    {
        /// <summary>
        /// Hash of the offer (only if this transaction, is a response from <see cref="SwitcheoClient.OrdersEndpoint"/>)
        /// </summary>
        [JsonOptionalProperty]
        [JsonProperty("offerHash")]
        public string OfferHash { get; set; }
        
        /// <summary>
        /// Hash of transaction
        /// </summary>
        [JsonProperty("hash")]
        public string Hash { get; set; }

        /// <summary>
        /// Sha256 provided for convenience (not used)
        /// </summary>
        [JsonProperty("sha256")]
        public string Sha256 { get; set; }

        /// <summary>
        /// Invocation parameters of transaction (only if this transaction, is a response from <see cref="SwitcheoClient.OrdersEndpoint"/>)
        /// </summary>
        [JsonOptionalProperty]
        [JsonProperty("invoke")]
        public SwitcheoScriptParams InvocationParameters { get; set; }

        /// <summary>
        /// Type of transaction
        /// </summary>
        [JsonProperty("type")]
        public int Type { get; set; }

        /// <summary>
        /// Version of transaction
        /// </summary>
        [JsonProperty("version")]
        public int Version { get; set; }

        /// <summary>
        /// Attribute(s) of transaction
        /// </summary>
        [JsonProperty("attributes")]
        public SwitcheoTransactionAttribute[] Attributes { get; set; }

        /// <summary>
        /// Input(s) of transaction
        /// </summary>
        [JsonProperty("inputs")]
        public SwitcheoTransactionInput[] Inputs { get; set; }

        /// <summary>
        /// Output(s) of transaction
        /// </summary>
        [JsonProperty("outputs")]
        public SwitcheoTransactionOutput[] Outputs { get; set; }

        /// <summary>
        /// Script(s) of transaction
        /// </summary>
        [JsonProperty("scripts")]
        public string[] Scripts { get; set; }

        /// <summary>
        /// Script of transaction
        /// </summary>
        [JsonProperty("script")]
        public string Script { get; set; }

        /// <summary>
        /// Gas cost of transaction
        /// </summary>
        [JsonProperty("gas")]
        public decimal Gas { get; set; }

        /// <summary>
        /// Serialize a <see cref="SwitcheoTransaction"/> to <see cref="NeoModules.NEP6.Transactions.SignedTransaction"/>
        /// </summary>
        /// <returns></returns>
        public SignedTransaction ToSignedTransaction()
        {
            SignedTransaction signedTransaction = new SignedTransaction
            {
                Type = (TransactionType)this.Type,
                Version = Convert.ToByte(this.Version),

                Attributes = new TransactionAttribute[this.Attributes.Length],
                Inputs = new SignedTransaction.Input[this.Inputs.Length],
                Outputs = new SignedTransaction.Output[this.Outputs.Length],

                Script = this.Script.HexToBytes(),
                Gas = this.Gas
            };

            for (int i = 0; i < this.Attributes.Length; i++)
            {
                signedTransaction.Attributes[i] = new TransactionAttribute()
                {
                    Data = this.Attributes[i].Data.HexToBytes(),
                    Usage = (TransactionAttributeUsage)this.Attributes[i].Usage
                };
            }

            for (int i = 0; i < this.Inputs.Length; i++)
            {
                signedTransaction.Inputs[i] = new SignedTransaction.Input()
                {
                    PrevHash = this.Inputs[i].PrevHash.HexToBytes().Reverse().ToArray(),
                    PrevIndex = this.Inputs[i].PrevIndex
                };
            }

            for (int i = 0; i < this.Outputs.Length; i++)
            {
                signedTransaction.Outputs[i] = new SignedTransaction.Output()
                {
                    AssetId = this.Outputs[i].Asset.Id.HexToBytes().Reverse().ToArray(),
                    ScriptHash = this.Outputs[i].ScriptHash.HexToBytes().Reverse().ToArray(),
                    Value = this.Outputs[i].Value
                };
            }

            return signedTransaction;
        }

        public override string ToString()
        {
            return string.Format("{{ OfferHash : {0}, Hash : {1}, Sha256 : {2}, InvocationParameters : {3}, Type : {4}, Version : {5}, Attributes : {6}, Inputs : {7}, Outputs : {8}, Scripts : {9}, Script : {10}, Gas : {11} }}",
                this.OfferHash, this.Hash, this.Sha256,
                this.InvocationParameters != null ? this.InvocationParameters.ToString() : "null",
                this.Type, this.Version,
                this.Attributes != null ? $"(Length : {this.Attributes.Length})" : "null",
                this.Inputs != null ? $"(Length : {this.Inputs.Length})" : "null",
                this.Outputs != null ? $"(Length : {this.Outputs.Length})" : "null",
                this.Scripts != null ? $"(Length : {this.Scripts.Length})" : "null",
                this.Script, this.Gas);
        }
    }
}
