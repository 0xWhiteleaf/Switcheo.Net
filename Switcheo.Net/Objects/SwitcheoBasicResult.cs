using Newtonsoft.Json;

namespace Switcheo.Net.Objects
{
    /// <summary>
    /// Represent a "basic" result from Switcheo API
    /// </summary>
    public class SwitcheoBasicResult
    {
        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonIgnore]
        public bool ActionSucceeded
        {
            get
            {
                return this.Result == "ok";
            }
        }
    }
}
