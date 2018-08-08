using System;

namespace Switcheo.Net.Attributes
{
    public class SymbolAttribute : Attribute
    {
        public string Symbol { get; set; }

        public SymbolAttribute(string symbol)
        {
            this.Symbol = symbol;
        }
    }
}
