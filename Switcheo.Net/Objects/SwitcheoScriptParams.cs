using Newtonsoft.Json;
using Switcheo.Net.Converters;

namespace Switcheo.Net.Objects
{
    /// <summary>
    /// Information about parameters of a script
    /// </summary>
    public class SwitcheoScriptParams
    {
        [JsonProperty("scriptHash")]
        public string ScriptHash { get; set; }

        [JsonProperty("operation")]
        [JsonConverter(typeof(OperationTypeConverter))]
        public OperationType Operation { get; set; }

        [JsonProperty("args")]
        public string[] Args { get; set; }

        public override string ToString()
        {
            return string.Format("{{ ScriptHash : {0}, Operation : {1}, Args : {2} }}", this.ScriptHash, this.Operation.ToString(),
                this.Args != null ? $"(Length : {this.Args.Length})" : "null");
        }
    }
}
