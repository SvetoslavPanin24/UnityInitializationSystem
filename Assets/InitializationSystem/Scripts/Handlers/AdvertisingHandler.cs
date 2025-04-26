using System;
namespace InitializationSystem
{
    public abstract class AdvertisingHandler : Handler
    { 
        // Banner methods
        public abstract void ShowBanner();
        public abstract void HideBanner();
        public abstract void DestroyBanner();

        // Interstitial methods
        public abstract void RequestInterstitial();
        public abstract void ShowInterstitial(Action<bool> callback);
        public abstract bool IsInterstitialLoaded { get; }

        // Rewarded video methods
        public abstract void RequestRewardedVideo();
        public abstract void ShowRewardedVideo(Action<bool> callback);
        public abstract bool IsRewardedVideoLoaded { get; }

        public virtual void SetGDPR(bool state) { }

        //Callbacks
        public abstract event Action OnInterstitialSuccess;
        public abstract event Action OnRewardedVideoSuccess;
        public abstract event Action OnRewardedVideoFailed;
    }
}
