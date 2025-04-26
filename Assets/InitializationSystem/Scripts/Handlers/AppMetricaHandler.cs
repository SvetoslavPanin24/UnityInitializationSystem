#if MODULE_Appmetrica
using Io.AppMetrica;
using UnityEngine;
using System.Collections.Generic;

namespace InitializationSystem
{
    [CreateAssetMenu(fileName = "AppMetricaHandler", menuName = "InitializationSystem/Handlers/AppMetricaHandler")]

    public class AppMetricaHandler : AnalyticsHandler
    {
        [SerializeField] private AppMetricaContainer _container;

        public override void Init()
        {
            var config = new AppMetricaConfig(_container.ApiKey)
            {
                FirstActivationAsUpdate = !IsFirstLaunch()
            };

            AppMetrica.Activate(config);
        }

        private bool IsFirstLaunch()
        {
            bool isFirstLaunch = !PlayerPrefs.HasKey(FirstLaunchKey);
            if (isFirstLaunch)
            {
                PlayerPrefs.SetInt(FirstLaunchKey, 1);
                PlayerPrefs.Save();
            }
            return isFirstLaunch;
        }

        public override void LogEvent(string name) => AppMetrica.ReportEvent(name);
        public override void LogEvent(string name, params LogParameter[] parameters)
        {

            var eventParams = new Dictionary<string, object>();
            foreach (var param in parameters)
                eventParams[param.Key] = param.Value;
            string jsonParams = JsonUtility.ToJson(eventParams);
            AppMetrica.ReportEvent(name, jsonParams);
        }
    }
}
#endif