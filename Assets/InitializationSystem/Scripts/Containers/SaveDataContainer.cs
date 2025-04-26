using System;
using UnityEngine;

namespace InitializationSystem
{
    [Serializable]
    public class SavedDataContainer
    {
        [SerializeField] private int hash;
        public int Hash => hash;

        [SerializeField] private string json;
        public bool Restored { get; set; }

        [System.NonSerialized] private ISaveObject saveObject;
        public ISaveObject SaveObject => saveObject;

        public SavedDataContainer(int hash, ISaveObject saveObject)
        {
            this.hash = hash;
            this.saveObject = saveObject;
            Restored = true;
        }

        public void Flush()
        {
            if (saveObject != null) saveObject.Flush();
            if (Restored) json = JsonUtility.ToJson(saveObject);
        }

        public void Restore<T>() where T : ISaveObject
        {
            saveObject = JsonUtility.FromJson<T>(json);
            Restored = true;
        }
    }
}