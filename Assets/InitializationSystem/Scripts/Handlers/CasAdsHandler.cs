#if MODULE_CAS
using System;
using UnityEngine;
using CAS;

namespace InitializationSystem
{
    [CreateAssetMenu(fileName = "Cas handler", menuName = "InitializationSystem/Handlers/Cas handler")]
    public class CasAdsHandler : AdvertisingHandler
    {
        private readonly IMediationManager mediationManager;
        private RewardedVideoCallback rewardCallback;

        public override event Action OnInterstitialSuccess;
        public override event Action OnRewardedVideoSuccess;
        public override event Action OnRewardedVideoFailed;

        public override void Init()
        {
            mediationManager = MobileAds.BuildManager().Build();

            ShowBanner();
            mediationManager.OnInterstitialAdShown += () =>
            {
                Time.timeScale = 0;
                AudioListener.volume = 0;
            };

            mediationManager.OnInterstitialAdClosed += () =>
            {
                Time.timeScale = 1;
                AudioListener.volume = 1;
            };

            mediationManager.OnRewardedAdShown += () =>
            {
                Time.timeScale = 0;
                AudioListener.volume = 0;
            };

            mediationManager.OnRewardedAdClosed += () =>
            {
                Time.timeScale = 1;
                AudioListener.volume = 1;
            };

            mediationManager.OnRewardedAdCompleted += () =>
            {
                rewardCallback?.Invoke(true);
                rewardCallback = null;
            };

            mediationManager.OnAppOpenAdImpression += metaData =>
            {
           /*     Io.AppMetrica.AppMetrica.ReportEvent(
                    "AdEvents", JsonConvert.SerializeObject(
                        new Dictionary<string, object> { { "ad_name", metaData.type } }
                      ));*/
            };
        }

        public override void ShowBanner() => mediationManager?.GetAdView(AdSize.Banner)?.SetActive(true);
        public override void HideBanner() => mediationManager?.GetAdView(AdSize.Banner)?.SetActive(false);
        public override void DestroyBanner() => HideBanner();
        public override void RequestInterstitial() => mediationManager?.LoadAd(AdType.Interstitial);
        public override bool IsInterstitialLoaded => mediationManager != null && mediationManager.IsReadyAd(AdType.Interstitial);
        public override void RequestRewardedVideo() => mediationManager?.LoadAd(AdType.Rewarded);

        public override void ShowInterstitial(Action<bool> callback)
        {
            if (mediationManager == null || !IsInterstitialLoaded)
            {
                callback?.Invoke(false);
                return;
            }
            mediationManager.ShowAd(AdType.Interstitial);
            callback?.Invoke(true);
        }

        public override void ShowRewardedVideo(Action<bool> callback)
        {
            if (mediationManager == null || !IsRewardedVideoLoaded)
            {
                callback?.Invoke(false);
                return;
            }
            mediationManager.ShowAd(AdType.Rewarded, result => callback?.Invoke(result == ShowResult.Finished));
        }

        public override bool IsRewardedVideoLoaded => mediationManager != null && mediationManager.IsReadyAd(AdType.Rewarded);
    }
}
#endif