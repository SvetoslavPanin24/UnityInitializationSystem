#if Module_YG
using System;
using UnityEngine;
using YG;

namespace InitializationSystem
{
    [CreateAssetMenu(fileName = "YG2 handler", menuName = "InitializationSystem/Handlers/YG2 handler")]
    public class YG2AdsHandler : AdvertisingHandler
    {
        private Action<bool> rewardCallback;
        private string currentRewardID = "Kludge";

        public override event Action OnInterstitialSuccess;
        public override event Action OnRewardedVideoSuccess;
        public override event Action OnRewardedVideoFailed;

        public override void Init()
        {
            YG2.onOpenInterAdv += OnInterstitialOpen;
            YG2.onCloseInterAdv += OnInterstitialClose;
            YG2.onRewardAdv += OnRewardedAdv;
            YG2.onOpenRewardedAdv += OnRewardedVideoOpen;
            YG2.onCloseRewardedAdv += OnRewardedVideoClose;         
        }   

        #region Interstitial Callbacks
        private void OnInterstitialOpen()
        {
            Time.timeScale = 0;
            AudioListener.volume = 0;
        }

        private void OnInterstitialClose()
        {
            Time.timeScale = 1;
            AudioListener.volume = 1;
            OnInterstitialSuccess?.Invoke();
        }

        #endregion

        #region Rewarded Video Callbacks
        private void OnRewardedVideoOpen()
        {
            Time.timeScale = 0;
            AudioListener.volume = 0;
        }

        private void OnRewardedVideoClose()
        {
            Time.timeScale = 1;
            AudioListener.volume = 1;            
        }
        #endregion

        #region Interstitial Ads
        public override void RequestInterstitial() => YG2.optionalPlatform.LoadInterAdv();
        public override bool IsInterstitialLoaded => true;

        public override void ShowInterstitial(Action<bool> callback)
        {
            if (!IsInterstitialLoaded)
            {
                callback?.Invoke(false);
                return;
            }

            YG2.InterstitialAdvShow();
            callback?.Invoke(true);
        }
        #endregion

        #region Banner Ads
        public override void ShowBanner() => YG2.StickyAdActivity(true);
        public override void DestroyBanner() => HideBanner();
        public override void HideBanner() => YG2.StickyAdActivity(false);
        #endregion

        #region Rewarded Video Ads
        public override void RequestRewardedVideo() => YG2.optionalPlatform.LoadRewardedAdv();
        public override bool IsRewardedVideoLoaded => true;

        public override void ShowRewardedVideo(Action<bool> callback)
        {
            rewardCallback = callback;
            YG2.RewardedAdvShow(currentRewardID, null);
        } 
      
        private void OnRewardedAdv(string rewardId)
        {
            bool success = !string.IsNullOrEmpty(currentRewardID) && rewardId == currentRewardID;

            rewardCallback?.Invoke(success);
            if (success)                            
               OnRewardedVideoSuccess?.Invoke();
            else
                OnRewardedVideoFailed.Invoke();

            rewardCallback = null;
            currentRewardID = null;
        }
        #endregion
    }
}
#endif