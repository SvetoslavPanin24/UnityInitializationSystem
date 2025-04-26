using UnityEngine;

namespace InitializationSystem
{
    public class AdsDummyController : MonoBehaviour
    {
        [SerializeField] private GameObject bannerObject;
        [SerializeField] private GameObject interstitialObject;
        [SerializeField] private GameObject rewardedVideoObject;

        private RectTransform bannerRectTransform;
        private AdsDummyController dummyInstance;

        private void Awake()
        {
            bannerRectTransform = bannerObject.GetComponent<RectTransform>();
            DontDestroyOnLoad(gameObject);
        }

        public void Init(BannerPosition bannerPosition)
        {            
            switch (bannerPosition)
            {
                case BannerPosition.Bottom:
                    bannerRectTransform.pivot = new Vector2(0.5f, 0.0f);
                    bannerRectTransform.anchorMin = new Vector2(0.0f, 0.0f);
                    bannerRectTransform.anchorMax = new Vector2(1.0f, 0.0f);
                    bannerRectTransform.anchoredPosition = Vector2.zero;
                    break;
                case BannerPosition.Top:
                    bannerRectTransform.pivot = new Vector2(0.5f, 1.0f);
                    bannerRectTransform.anchorMin = new Vector2(0.0f, 1.0f);
                    bannerRectTransform.anchorMax = new Vector2(1.0f, 1.0f);
                    bannerRectTransform.anchoredPosition = Vector2.zero;
                    break;
            } 
        }

        public void ShowBanner() => bannerObject.SetActive(true);
        public void HideBanner() => bannerObject.SetActive(false);

        public void ShowInterstitial() => interstitialObject.SetActive(true);
        public void ShowRewardedVideo() => rewardedVideoObject.SetActive(true);
    }
}
