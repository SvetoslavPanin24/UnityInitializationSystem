using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace InitializationSystem
{
    [CreateAssetMenu(fileName = "SaveModule", menuName = "InitializationSystem/Modules/Save Module")]
    public class SaveModule : InitModule
    {
        [SerializeField, Tooltip("Reference to the Save settings asset.")]
        private SaveSettings _saveSettings;
        [SerializeField] private List<SaveHandler> _saveHandlers;

        public override void StartInit()
        {
            if (_saveSettings.SystemLogs)
                Debug.Log($"[SaveModule]: Additional initialization for Save module '{this.name}'.");

            var handler = _saveHandlers?.FirstOrDefault(h => h != null && h.Enabled);
            
            // Получаем SaveService из ProjectContext вместо использования инъекции
            var saveService = ProjectContext.Instance.Container.Resolve<SaveService>();

            handler.Init();
            saveService.Init(handler, _saveSettings);
        }
    }
}
