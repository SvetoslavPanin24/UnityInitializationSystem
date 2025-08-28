using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace InitializationSystem
{
    [CreateAssetMenu(fileName = "AdsModule", menuName = "InitializationSystem/Modules/Ads Module")]
    public class AdsModule : InitModule
    {
        [SerializeField, Tooltip("Reference to the Ads _settings asset.")]
        private AdsSettings _adsSettings;
        [SerializeField]private List<AdvertisingHandler> advertisingHandlers = new();  

        public override void StartInit() 
        {
            if(_adsSettings.SystemLogs)
            Debug.Log($"[AdsModule]: Additional initialization for Ads module '{this.name}'.");

            var handler = advertisingHandlers?.FirstOrDefault(h => h != null && h.Enabled);
            
            // Получаем AdsService из ProjectContext вместо использования инъекции
            var adsService = ProjectContext.Instance.Container.Resolve<AdsService>();

            handler.Init();
            adsService.Init(handler, _adsSettings);
        }
    }
}
