using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static UnityEngine.Rendering.RayTracingAccelerationStructure;

namespace InitializationSystem
{
    [CreateAssetMenu(fileName = "AnalyticsModule", menuName = "InitializationSystem/Modules/AnalyticsModule")]
    public class AnalyticsModule : InitModule
    {
        [SerializeField, Tooltip("Reference to the AnalyticsSettings _settings asset.")]
        private AnalyticsSettings _analyticsSettings;
        [SerializeField] private List<AnalyticsHandler> _analyticsHandlers;

        public override void StartInit()
        {
            if (_analyticsSettings.SystemLogs)
                Debug.Log($"[AnalyticsModule]: Additional initialization for Analytics Module '{this.name}'.");

            // Получаем AnalyticsService из ProjectContext вместо использования инъекции
            var analyticsService = ProjectContext.Instance.Container.Resolve<AnalyticsService>();
            
            analyticsService.Init(_analyticsHandlers, _analyticsSettings);
        }
    }
}
