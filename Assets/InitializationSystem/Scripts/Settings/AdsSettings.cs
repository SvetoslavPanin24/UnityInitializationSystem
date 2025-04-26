using UnityEngine;

namespace InitializationSystem
{
    [CreateAssetMenu(fileName = "Ads Settings", menuName = "InitializationSystem/Settings/Ads Settings")]
    public class AdsSettings : ScriptableObject
    {
        [Header("Debug")]
        [Space]
        [Tooltip("Enable system logs for debugging ads.")]
        [SerializeField] private bool systemLogs = false;
        public bool SystemLogs => systemLogs;

        [Header("Ads Settings")]
        [Space]       
        [Tooltip("Delay in seconds before interstitial appearings on first game launch.")]
        [SerializeField] float interstitialFirstStartDelay = 40f;
        public float InterstitialFirstStartDelay => interstitialFirstStartDelay;
 
        [Tooltip("Delay in seconds before interstitial appearings.")]
        [SerializeField] float interstitialStartDelay = 40f;
        public float InterstitialStartDelay => interstitialStartDelay;

        [Tooltip("Delay in seconds between interstitial appearings.")]
        [SerializeField] float interstitialShowingDelay = 30f;
        public float InterstitialShowingDelay => interstitialShowingDelay;

        [Space]
        [SerializeField] bool autoShowInterstitial;
        public bool AutoShowInterstitial => autoShowInterstitial;
    } 
}
