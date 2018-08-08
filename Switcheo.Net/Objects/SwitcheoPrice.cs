namespace Switcheo.Net.Objects
{
    /// <summary>
    /// The price of a pair
    /// </summary>
    public class SwitcheoPrice
    {
        /// <summary>
        /// The pair the price is for
        /// </summary>
        public string Pair { get; set; }

        /// <summary>
        /// The price of the pair
        /// </summary>
        public decimal Price { get; set; }

        public override string ToString()
        {
            return string.Format("{{ Pair : {0}, Price : {1} }}", this.Pair, this.Price);
        }
    }
}
