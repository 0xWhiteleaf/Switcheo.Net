namespace Switcheo.Net.Objects
{
    /// <summary>
    /// Information about a contract
    /// For more informations about contracts visit <a href="https://docs.switcheo.network/#contracts"/>#contracts</a>
    /// You can also take a look at <a href="https://docs.switcheo.network/#address"/>#address</a>
    /// </summary>
    public class SwitcheoContract
    {
        /// <summary>
        /// Version of contract (V1, V2, ...)
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Hash of contract according to corresponding blockchain
        /// </summary>
        public string Hash { get; set; }

        public override string ToString()
        {
            return string.Format("{{ Version : {0}, Hash : {1} }}", this.Version, this.Hash);
        }
    }
}
