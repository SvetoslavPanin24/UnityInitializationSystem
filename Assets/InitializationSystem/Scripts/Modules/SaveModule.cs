using System.Collections.Generic;
using System.Linq;
using UnityEngine;
 
namespace InitializationSystem
{
    [CreateAssetMenu(fileName = "SaveModule", menuName = "InitializationSystem/Modules/Save Module")]
    public class SaveModule : InitModule
    {
        [SerializeField, Tooltip("Reference to the Save settings asset.")]
        private SaveSettings _saveSettings;
        public SaveSettings SaveSettings => _saveSettings;

        [SerializeField]
        private List<SaveHandler> _saveHandlers;       

        public override void StartInit() 
        {
            if(_saveSettings.SystemLogs)
            Debug.Log($"[SaveModule]: Additional initialization for Save module '{this.name}'.");

            var handler = _saveHandlers?.FirstOrDefault(h => h != null && h.Enabled);

            handler.Init();
            SaveManager.Init(handler, _saveSettings);
        } 
    }
}
