using UnityEngine;
namespace InitializationSystem
{

    public abstract class Handler : ScriptableObject
    {
        [field: SerializeField] public bool Enabled { get; protected set; }
        [HideInInspector] public bool IsInitialized = false;

        public abstract void Init();
    }
}