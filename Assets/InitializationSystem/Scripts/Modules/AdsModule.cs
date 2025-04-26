using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InitializationSystem
{
    [CreateAssetMenu(fileName = "AdsModule", menuName = "InitializationSystem/Modules/Ads Module")]
    public class AdsModule : InitModule
    {
        [SerializeField, Tooltip("Reference to the Ads _settings asset.")]
        private AdsSettings _adsSettings;
        public AdsSettings AdsSettings => _adsSettings;

        [SerializeField]private List<AdvertisingHandler> advertisingHandlers = new();  

        public override void StartInit() 
        {
            if(_adsSettings.SystemLogs)
            Debug.Log($"[AdsModule]: Additional initialization for Ads module '{this.name}'.");

            var handler = advertisingHandlers?.FirstOrDefault(h => h != null && h.Enabled);

            handler.Init();
            AdsManager.Init(handler, _adsSettings);
        }
    }
}
