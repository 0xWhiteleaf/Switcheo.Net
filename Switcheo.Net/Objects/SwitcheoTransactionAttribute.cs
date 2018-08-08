using Newtonsoft.Json;

namespace Switcheo.Net.Objects
{
    public class SwitcheoTransactionAttribute
    {
        /// <summary>
        /// Usage code
        /// </summary>
        [JsonProperty("usage")]
        public int Usage { get; set; }

        /// <summary>
        /// Data as string
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }

        public override string ToString()
        {
            return string.Format("{{ Usage : {0}, Data : {1} }}", this.Usage, this.Data);
        }
    }
}
