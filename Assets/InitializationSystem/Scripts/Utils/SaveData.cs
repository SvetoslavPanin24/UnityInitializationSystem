using System;
using System.Collections.Generic;
using UnityEngine;

namespace InitializationSystem
{
    [Serializable]
    public class SaveData
    {
        [SerializeField] public string buildId;

        [SerializeField] private SavedDataContainer[] saveObjects;
        private List<SavedDataContainer> saveObjectsList;

        [SerializeField] private float gameTime;
        public float GameTime => gameTime + (Time - lastFlushTime);

        [SerializeField] private DateTime lastExitTime;
        public DateTime LastExitTime => lastExitTime;

        private float lastFlushTime = 0;

        public float Time { get; set; }

        public void Init(float time)
        {
            if (saveObjects == null)
                saveObjectsList = new List<SavedDataContainer>();
            else
                saveObjectsList = new List<SavedDataContainer>(saveObjects);

            for (int i = 0; i < saveObjectsList.Count; i++)
                saveObjectsList[i].Restored = false;

            Time = time;
            lastFlushTime = Time;
        }

        public void Flush()
        {
            saveObjects = saveObjectsList.ToArray();

            for (int i = 0; i < saveObjectsList.Count; i++)
            {
                var saveObject = saveObjectsList[i];
                saveObject.Flush();
            }

            gameTime += Time - lastFlushTime;
            lastFlushTime = Time;
            lastExitTime = DateTime.Now;
        }

        public T GetSaveObject<T>(int hash) where T : ISaveObject, new()
        {
            var container = saveObjectsList.Find((container) => container.Hash == hash);

            if (container == null)
            {
                var saveObject = new T();
                container = new SavedDataContainer(hash, saveObject);
                saveObjectsList.Add(container);
            }
            else if (!container.Restored)
                container.Restore<T>();

            return (T)container.SaveObject;
        }

        public T GetSaveObject<T>(string uniqueName) where T : ISaveObject, new() => GetSaveObject<T>(uniqueName.GetHashCode());

        public void ClearSaveData()
        {
            saveObjectsList.Clear();
            saveObjects = null;
            gameTime = 0;
        }
    }
}