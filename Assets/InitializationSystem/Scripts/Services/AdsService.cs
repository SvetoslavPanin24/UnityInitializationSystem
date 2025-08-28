using System;
using System.Threading;
using System.Threading.Tasks;

namespace InitializationSystem
{
    public class AdsService
    {
        private AdsSettings _settings;
        private AdvertisingHandler _advertisingHandler;
        private CancellationTokenSource _autoShowCts;

        public event Action OnRewardedSuccess;
        public event Action OnInterstitialSuccess;

        public void Init(AdvertisingHandler advertisingHandler, AdsSettings adsData)
        {
            if (advertisingHandler == null || adsData == null) return;

            _advertisingHandler = advertisingHandler;
            _settings = adsData;

            _advertisingHandler.OnRewardedVideoSuccess += HandleRewardedSuccess;
            _advertisingHandler.OnInterstitialSuccess += HandleInterstitialSuccess;
 
            if (_settings.AutoShowInterstitial && _autoShowCts == null)
            {
                _autoShowCts = new CancellationTokenSource();
                _ = AutoShowLoopAsync(_autoShowCts.Token);
            }
        }

        public void StopAutoShowInterstitial()
        {
            if (_autoShowCts != null)
            {
                _autoShowCts.Cancel();
                _autoShowCts.Dispose();
                _autoShowCts = null;

                if (_settings.SystemLogs)
                    UnityEngine.Debug.Log("Auto-show interstitial stopped.");
            }
        }

        private void HandleRewardedSuccess() => OnRewardedSuccess?.Invoke();
        private void HandleInterstitialSuccess() => OnInterstitialSuccess?.Invoke();

        public void ShowRewarded(Action onSuccess, Action onFail)
        {
            _advertisingHandler.ShowRewardedVideo(result =>
            {
                if (result)
                    onSuccess?.Invoke();
                else
                    onFail?.Invoke();
            });
        }

        public void ShowInterstitial() => _advertisingHandler.ShowInterstitial(null);
        public void ShowBanner() => _advertisingHandler.ShowBanner();
        public void HideBanner() => _advertisingHandler.HideBanner();
 
        private async Task AutoShowLoopAsync(CancellationToken token)
        {
            try
            {
                // Delay before first show on initial launch
                await Task.Delay(TimeSpan.FromSeconds(_settings.InterstitialFirstStartDelay), token);
                ShowInterstitial();

                // Then show at regular intervals
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(_settings.InterstitialShowingDelay), token);
                    if (token.IsCancellationRequested) break;
                    ShowInterstitial();
                }
            }
           
            catch (Exception ex)
            {
                if (_settings.SystemLogs)
                    UnityEngine.Debug.LogError($"AutoShowLoopAsync error: {ex}");
            }
        } 
    }
}
