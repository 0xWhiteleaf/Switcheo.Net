using CryptoExchange.Net.Converters;
using Switcheo.Net.Helpers;
using Switcheo.Net.Objects;
using System.Collections.Generic;
using System.Linq;

namespace Switcheo.Net.Converters
{
    public class EventTypeConverter : BaseConverter<EventType>
    {
        public EventTypeConverter() : this(true) { }
        public EventTypeConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<EventType, string> Mapping => new Dictionary<EventType, string>()
        {
            { EventType.Deposit, "deposit" },
            { EventType.Withdrawal, "withdrawal" }
        };

        public EventType GetEventTypeFromString(string value)
        {
            EventType eventType = EventType.Unknown;

            var pair = this.Mapping.FirstOrDefault(m => m.Value == value);
            if (!pair.IsDefault())
                eventType = pair.Key;

            return eventType;
        }
    }
}
