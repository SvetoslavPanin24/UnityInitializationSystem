using UnityEngine;

namespace InitializationSystem
{
    /// <summary>
    /// Base class for initialization modules.
    /// </summary>
    public abstract class InitModule : ScriptableObject 
    {
        [SerializeField, HideInInspector]
        protected string moduleName = "Default Module";
        public abstract void StartInit();
        protected virtual void OnValidate() => moduleName = this.name;
    }
}
