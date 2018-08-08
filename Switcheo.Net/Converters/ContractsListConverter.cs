using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Switcheo.Net.Helpers;
using Switcheo.Net.Objects;
using System;
using System.Linq;

namespace Switcheo.Net.Converters
{
    public class ContractsListConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            SwitcheoContractsList contractsList = (SwitcheoContractsList)value;

            if (contractsList.NeoContracts != null && contractsList.NeoContracts.Length > 0)
            {
                writer.WritePropertyName(BlockchainType.Neo.GetSymbol());
                writer.WriteStartObject();

                foreach (SwitcheoContract neoContract in contractsList.NeoContracts)
                {
                    writer.WritePropertyName(neoContract.Version);
                    writer.WriteValue(neoContract.Hash);
                }

                writer.WriteEndObject();
            }
            if (contractsList.QtumContracts != null && contractsList.QtumContracts.Length > 0)
            {
                writer.WritePropertyName(BlockchainType.Qtum.GetSymbol());
                writer.WriteStartObject();

                foreach (SwitcheoContract qtumContract in contractsList.QtumContracts)
                {
                    writer.WritePropertyName(qtumContract.Version);
                    writer.WriteValue(qtumContract.Hash);
                }

                writer.WriteEndObject();
            }
            if (contractsList.EthereumContracts != null && contractsList.EthereumContracts.Length > 0)
            {
                writer.WritePropertyName(BlockchainType.Ethereum.GetSymbol());
                writer.WriteStartObject();

                foreach (SwitcheoContract ethContract in contractsList.EthereumContracts)
                {
                    writer.WritePropertyName(ethContract.Version);
                    writer.WriteValue(ethContract.Hash);
                }

                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            SwitcheoContractsList contractsList = new SwitcheoContractsList();
            
            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject rootObject = JObject.Load(reader);
                
                if (rootObject.ContainsKey(BlockchainType.Neo.GetSymbol()))
                {
                    JToken neoContracts = rootObject.GetValue(BlockchainType.Neo.GetSymbol());

                    contractsList.NeoContracts = new SwitcheoContract[neoContracts.Children().Count()];
                    int tabIndex = 0;

                    foreach (JProperty neoContract in neoContracts.Children())
                    {
                        contractsList.NeoContracts[tabIndex] = new SwitcheoContract() { Version = neoContract.Name, Hash = neoContract.Value.ToString() };
                        tabIndex++;
                    }
                }
                if (rootObject.ContainsKey(BlockchainType.Qtum.GetSymbol()))
                {
                    JToken qtumContracts = rootObject.GetValue(BlockchainType.Qtum.GetSymbol());

                    contractsList.QtumContracts = new SwitcheoContract[qtumContracts.Children().Count()];
                    int tabIndex = 0;

                    foreach (JProperty qtumContract in qtumContracts.Children())
                    {
                        contractsList.QtumContracts[tabIndex] = new SwitcheoContract() { Version = qtumContract.Name, Hash = qtumContract.Value.ToString() };
                        tabIndex++;
                    }
                }
                if (rootObject.ContainsKey(BlockchainType.Ethereum.GetSymbol()))
                {
                    JToken ethContracts = rootObject.GetValue(BlockchainType.Ethereum.GetSymbol());

                    contractsList.EthereumContracts = new SwitcheoContract[ethContracts.Children().Count()];
                    int tabIndex = 0;

                    foreach (JProperty ethContract in ethContracts.Children())
                    {
                        contractsList.EthereumContracts[tabIndex] = new SwitcheoContract() { Version = ethContract.Name, Hash = ethContract.Value.ToString() };
                        tabIndex++;
                    }
                }
            }

            return contractsList;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SwitcheoContractsList);
        }
    }
}
