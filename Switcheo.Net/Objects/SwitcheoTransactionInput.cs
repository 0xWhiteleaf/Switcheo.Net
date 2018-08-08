using Newtonsoft.Json;

namespace Switcheo.Net.Objects
{
    public class SwitcheoTransactionInput
    {
        /// <summary>
        /// Previous hash
        /// </summary>
        [JsonProperty("prevHash")]
        public string PrevHash { get; set; }

        /// <summary>
        /// Previous index
        /// </summary>
        [JsonProperty("prevIndex")]
        public uint PrevIndex { get; set; }

        public override string ToString()
        {
            return string.Format("{{ PrevHash : {0}, PrevIndex : {1} }}", this.PrevHash, this.PrevIndex);
        }
    }
}
