using UnityEngine;

namespace InitializationSystem
{
    /// <summary>
    /// MonoBehaviour that initializes the project using ProjectInitSettings.
    /// </summary>
 
    public class Initializer : MonoBehaviour
    {
        [SerializeField] private ProjectInitSettings _initSettings;      
        public static GameObject InitialiserGameObject { get; private set; }
        public static ProjectInitSettings InitSettings { get; private set; }
        public static bool IsInitialised { get; private set; }
        public static bool IsStartInitialised { get; private set; }

        private void Awake()
        {
            if (IsInitialised) return;

            IsInitialised = true;
            InitSettings = _initSettings;
             
            InitialiserGameObject = gameObject;
            DontDestroyOnLoad(gameObject);

            _initSettings?.Initialise();
        }

        private void Start() => Initialise(true);

        public void Initialise(bool loadScene)
        {
            if (IsStartInitialised) return;
            
                _initSettings?.Initialise();
                IsStartInitialised = true;
                if (loadScene)
                {
                    Debug.Log("Loading main scene...");
                    // Scene loading logic here.
                }
                else
                {
                    Debug.Log("Simple load.");
                }
             
        }

        private void OnDestroy() => IsInitialised = false;
    }
}
