using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VL.Lib.UI
{
    public static class GlobalEventRegistry
    {
        static class EventRegistry<TEvent>
        {
            public static Dictionary<string, TEvent> Events = new Dictionary<string, TEvent>();
        }

        public static void AddEventSource<TEvent>(TEvent eventSource, string key)
        {
            EventRegistry<TEvent>.Events[key] = eventSource;
        }

        public static bool RemoveEventSource<TEvent>(TEvent eventSource, string key)
        {
            return EventRegistry<TEvent>.Events.Remove(key);
        }

        public static bool TryGetEventSource<TEvent>(string key, out TEvent eventSource)
        {
            return EventRegistry<TEvent>.Events.TryGetValue(key, out eventSource);
        }
    }
}
