namespace Switcheo.Net.Objects
{
    /// <summary>
    /// Information about a token
    /// </summary>
    public class SwitcheoToken
    {
        /// <summary>
        /// The token (or asset) symbol
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// The token (or asset) id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Decimals precision used for this token (or asset)
        /// </summary>
        public double Precision { get; set; }

        /// <summary>
        /// If the token is currently tradable or not
        /// </summary>
        public bool IsTradingActive { get; set; }

        public override string ToString()
        {
            return string.Format("{{ Symbol : {0}, Id : {1}, Precision : {2}, IsTradingActive : {3} }}",
                this.Symbol, this.Id, this.Precision, this.IsTradingActive);
        }
    }
}
