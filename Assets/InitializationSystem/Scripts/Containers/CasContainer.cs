using UnityEngine;

namespace InitializationSystem
{
    [CreateAssetMenu(fileName = "CasContainer", menuName = "InitializationSystem/Containers/Cas container")]
    public class CasContainer : ScriptableObject
    {
        [Tooltip("API key for CAS integration.")]
        public string ApiKey; 
    }
}
