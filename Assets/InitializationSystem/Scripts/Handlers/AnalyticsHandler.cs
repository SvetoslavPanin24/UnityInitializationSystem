 namespace InitializationSystem
{
    public abstract class AnalyticsHandler : Handler
    {
        protected const string FirstLaunchKey = "FirstLaunch";

        public abstract void LogEvent(string name);
        public abstract void LogEvent(string name, params LogParameter[] parameters);
    }
}
