using UnityEngine;

namespace InitializationSystem
{
    /// <summary>
    /// Project initialization _settings asset that holds a list of modules.
    /// </summary>
    [CreateAssetMenu(fileName = "ProjectInitSettings", menuName = "InitializationSystem/Settings/Project Init Settings")]
    public class ProjectInitSettings : ScriptableObject
    {
        [Header("Project Modules")]
        [SerializeField] private InitModule[] modules;
        public InitModule[] Modules => modules;

        public void Initialise()
        {
            if (modules == null) return;
            foreach (var module in modules)
                module?.StartInit();
        }        
    }
}
