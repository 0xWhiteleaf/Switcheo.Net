using Switcheo.Net.Attributes;
using System.Reflection;

namespace Switcheo.Net.Helpers
{
    public static class EnumsHelper
    {
        public static string GetSymbol<T>(this T source)
        {
            FieldInfo fi = source.GetType().GetField(source.ToString());

            SymbolAttribute[] attributes = (SymbolAttribute[])fi.GetCustomAttributes(
                typeof(SymbolAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Symbol;
            else return source.ToString();
        }
    }
}
