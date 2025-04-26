using UnityEngine;

namespace InitializationSystem
{
    [CreateAssetMenu(fileName = "Save Settings", menuName = "InitializationSystem/Settings/Save Settings")]
    public class SaveSettings : ScriptableObject
    {
        [Header("Save File Settings")]
        [SerializeField] private string saveFileName = "save";
        public string SaveFileName => saveFileName;

        [Header("Auto Save Settings")]
        [SerializeField] private bool useAutoSave = true;
        public bool UseAutoSave => useAutoSave;

        [SerializeField] private int saveDelay = 30;
        public int SaveDelay => saveDelay;

        [Header("First Launch Settings")]
        [SerializeField] private bool useClearSave = false;
        public bool UseClearSave => useClearSave;

        [Header("Debug")]
        [SerializeField] private bool systemLogs = false;
        public bool SystemLogs => systemLogs;
    }
}