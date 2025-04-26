using System.Collections.Generic;

namespace InitializationSystem
{
    public static class AnalyticsManager
    {
        private static AnalyticsSettings _settings;
        private static List<AnalyticsHandler> _analyticsHandlers;
        public static void Init(List<AnalyticsHandler> analyticsHandlers, AnalyticsSettings settings)
        {
            _settings = settings;
            _analyticsHandlers = analyticsHandlers;
        }

        public static void LogEvent(string name) =>
            _analyticsHandlers.ForEach(handler => handler.LogEvent(name));

        public static void LogEvent(string name, LogParameter[] parameters) =>
            _analyticsHandlers.ForEach(handler => handler.LogEvent(name, parameters));
    }
}
