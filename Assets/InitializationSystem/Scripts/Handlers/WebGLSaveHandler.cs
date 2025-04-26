using System.Runtime.InteropServices;
using UnityEngine;

namespace InitializationSystem
{
    [CreateAssetMenu(fileName = "WebGLSaveHandler", menuName = "InitializationSystem/Handlers/WebGL Save Handler")]
    public class WebGLSaveHandler : SaveHandler
    {
        public override void Init() => IsInitialized = true;

        public override SaveData LoadSave(string fileName)
        {

            string jsonObject = load(fileName);
            if (!string.IsNullOrEmpty(jsonObject))
            {
                try
                {
                    SaveData deserializedObject = JsonUtility.FromJson<SaveData>(jsonObject);
                    return deserializedObject;
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.Message);
                }
            }

            return new SaveData();
        }

        public override void SaveData(SaveData saveData, string fileName)
        {
            string jsonObject = JsonUtility.ToJson(saveData);
            save(fileName, jsonObject);
        }

        public override void DeleteSave(string fileName) => deleteItem(fileName);

        [DllImport("__Internal")]
        private static extern string load(string keyName);

        [DllImport("__Internal")]
        private static extern void save(string keyName, string data);

        [DllImport("__Internal")]
        private static extern void deleteItem(string keyName);
    }
}