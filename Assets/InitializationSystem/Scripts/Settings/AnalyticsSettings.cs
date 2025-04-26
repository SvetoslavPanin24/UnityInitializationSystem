using UnityEngine;

namespace InitializationSystem
{
    /// <summary>
    /// Main project _settings for analytics.
    /// </summary>
    [CreateAssetMenu(fileName = "Analytics Settings", menuName = "InitializationSystem/Settings/Analytics Settings")]
    public class AnalyticsSettings : ScriptableObject
    {
        [Header("Debug Settings")]
        [Tooltip("Enable system logs for debugging.")]
        [SerializeField] private bool systemLogs = false;
        public bool SystemLogs => systemLogs;
    }
}