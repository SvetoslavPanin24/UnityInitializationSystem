using System;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace InitializationSystem
{
    [CreateAssetMenu(fileName = "Dummy handler", menuName = "InitializationSystem/Handlers/Dummy handler")]
    public class DummyAdsHandler : AdvertisingHandler
    {
        [SerializeField] private DummyContainer _data;
        private AdsDummyController _controller;

        public override event Action OnInterstitialSuccess;
        public override event Action OnRewardedVideoSuccess;
        public override event Action OnRewardedVideoFailed;

        public override void Init()
        {
            _controller = Instantiate(_data.DummyPrefab)
                .GetComponent<AdsDummyController>();
            _controller.Init(_data.bannerPosition);
        }

        #region Interstitial Ads
        public override void RequestInterstitial() { }

        public override bool IsInterstitialLoaded => true;

        public override void ShowInterstitial(Action<bool> callback)
        {
            _controller.ShowInterstitial();
            callback?.Invoke(true);
        }
        #endregion

        #region Banner Ads
        public override void ShowBanner() => _controller.ShowBanner();
        public override void DestroyBanner() => _controller.HideBanner();
        public override void HideBanner() => _controller.HideBanner();
        #endregion

        #region Rewarded Video Ads
        public override void RequestRewardedVideo() { }
        public override bool IsRewardedVideoLoaded => true;

        public override void ShowRewardedVideo(Action<bool> callback)
        {
            _controller.ShowRewardedVideo();
            callback?.Invoke(true);
        }
        #endregion 
    }
}
