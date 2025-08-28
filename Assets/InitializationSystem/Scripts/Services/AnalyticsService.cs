using System.Collections.Generic;

namespace InitializationSystem
{
    public class AnalyticsService
    {
        private AnalyticsSettings _settings;
        private List<AnalyticsHandler> _analyticsHandlers;
        public void Init(List<AnalyticsHandler> analyticsHandlers, AnalyticsSettings settings)
        {
            _settings = settings;
            _analyticsHandlers = analyticsHandlers;
        }

        public void LogEvent(string name) =>
            _analyticsHandlers.ForEach(handler => handler.LogEvent(name));

        public void LogEvent(string name, LogParameter[] parameters) =>
            _analyticsHandlers.ForEach(handler => handler.LogEvent(name, parameters));
    }
}
