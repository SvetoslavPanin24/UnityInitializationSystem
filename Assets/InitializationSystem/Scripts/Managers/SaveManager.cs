using System;
using System.Threading;
using UnityEngine;

namespace InitializationSystem
{
    public static class SaveManager
    {
        private static SaveSettings _settings;
        private static SaveHandler _saveHandler;
        private static SaveData _saveData;
        private static bool _isSaveLoaded;
        private static bool _isSaveRequired;
        private static float _lastFlushTime;
        private static Timer _autoSaveTimer;

        public static bool IsSaveLoaded => _isSaveLoaded;
        public static float GameTime => _saveData?.GameTime ?? 0f;
        public static DateTime LastExitTime => _saveData?.LastExitTime ?? DateTime.Now;
        public static event Action OnSaveLoaded;

        public static void Init(SaveHandler saveHandler, SaveSettings settings)
        {
            if (saveHandler == null || settings == null) return;

            _saveHandler = saveHandler;
            _settings = settings;

            if (_settings.UseClearSave)
                InitClear(Time.time);
            else
                Load(Time.time);

            if (_settings.UseAutoSave)
                StartAutoSaveTimer();
        }

        private static void InitClear(float time)
        {
            _saveData = new SaveData();
            _saveData.Init(time);

            if (_settings.SystemLogs)
                Debug.Log("[SaveManager]: Created clear save!");

            _isSaveLoaded = true;
        }

        private static void Load(float time)
        {
            if (_isSaveLoaded)
                return;

            _saveData = _saveHandler.LoadSave(_settings.SaveFileName);
            _saveData.Init(time);
            _lastFlushTime = time;

            if (_settings.SystemLogs)
                Debug.Log("[SaveManager]: Save is loaded!");

            _isSaveLoaded = true;
            OnSaveLoaded?.Invoke();
        }

        public static T GetSaveObject<T>(int hash) where T : ISaveObject, new()
        {
            if (!_isSaveLoaded)
            {
                if (_settings.SystemLogs)
                    Debug.LogError("[SaveManager]: Save system has not been initialized");

                return default;
            }

            return _saveData.GetSaveObject<T>(hash);
        }

        public static T GetSaveObject<T>(string uniqueName) where T : ISaveObject, new() => GetSaveObject<T>(uniqueName.GetHashCode());

        public static void Save()
        {
            if (!_isSaveRequired || _saveData == null)
                return;

            _saveData.Flush();
            _saveHandler.SaveData(_saveData, _settings.SaveFileName);

            if (_settings.SystemLogs)
                Debug.Log("[SaveManager]: Game is saved!");

            _isSaveRequired = false;
        }

        public static void ForceSave()
        {
            if (_saveData == null) return;

            _saveData.Flush();
            _saveHandler.SaveData(_saveData, _settings.SaveFileName);

            if (_settings.SystemLogs)
                Debug.Log("[SaveManager]: Game is forcibly saved!");

            _isSaveRequired = false;
        }

        public static void MarkAsSaveIsRequired() => _isSaveRequired = true;

        public static void DeleteSaveFile()
        {
            _saveHandler.DeleteSave(_settings.SaveFileName);

            if (_saveData != null)
            {
                _saveData.ClearSaveData();
                _saveData = null;
            }

            _isSaveLoaded = false;
        }

        public static void UpdateTime(float time)
        {
            if (_saveData != null)
                _saveData.Time = time;
        }

        private static void StartAutoSaveTimer()
        {
            var interval = TimeSpan.FromSeconds(_settings.SaveDelay);
            _autoSaveTimer = new Timer(_ => Save(), null, interval, interval);
        }
    }
}
