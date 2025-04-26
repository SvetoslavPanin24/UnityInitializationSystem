using UnityEngine;

namespace InitializationSystem
{
    [CreateAssetMenu(fileName = "ConsoleHandler", menuName = "InitializationSystem/Handlers/ConsoleHandler")]
    public class ConsoleAnalyticsHandler : AnalyticsHandler
    {
        public override void Init(){}
        public override void LogEvent(string name) => Debug.Log($"[ConsoleAnalyticsHandler]: Event '{name}'.");
        public override void LogEvent(string name, params LogParameter[] parameters)
        {
            string log = $"[ConsoleAnalyticsHandler]: Event '{name}' with parameters: ";
            foreach (LogParameter param in parameters)
            {
                log += $"[{param.Key}:{param.Value}] ";
            }
            Debug.Log(log);
        }
    }
}
