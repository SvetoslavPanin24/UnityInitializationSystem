using UnityEngine;

namespace InitializationSystem
{
    [CreateAssetMenu(fileName = "AppMetrica container", menuName = "InitializationSystem/Containers/AppMetrica container")]
    public class AppMetricaContainer : ScriptableObject
    {
        [field: SerializeField,  Tooltip("API key for AppMetrica integration.")]
        public string ApiKey { get; private set; }
    }
}


