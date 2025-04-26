using UnityEngine;

namespace InitializationSystem
{
    [CreateAssetMenu(fileName = "DummyContainer", menuName = "InitializationSystem/Containers/DummyContainer")]
    public class DummyContainer : ScriptableObject
    {
        public GameObject DummyPrefab;    

        [Tooltip("Banner position for dummy ads.")]
        public BannerPosition bannerPosition = BannerPosition.Bottom; 
    }

    public enum BannerPosition
    {
        Bottom = 0,
        Top = 1
    }
}
