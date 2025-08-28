using System;
using System.Threading;
using UnityEngine;

namespace InitializationSystem
{
    public class SaveService
    {
        private SaveSettings _settings;
        private SaveHandler _saveHandler;
        private SaveData _saveData;
        private bool _isSaveLoaded;
        private bool _isSaveRequired;
        private float _lastFlushTime;
        private Timer _autoSaveTimer;

        public bool IsSaveLoaded => _isSaveLoaded;
        public float GameTime => _saveData?.GameTime ?? 0f;
        public DateTime LastExitTime => _saveData?.LastExitTime ?? DateTime.Now;
        public event Action OnSaveLoaded;

        public void Init(SaveHandler saveHandler, SaveSettings settings)
        {
            _saveHandler = saveHandler;
            _settings = settings;

            if (_settings.UseClearSave)
                InitClear(Time.time);
            else
                Load(Time.time);

            if (_settings.UseAutoSave)
                StartAutoSaveTimer();
        }

        private void InitClear(float time)
        {
            _saveData = new SaveData();
            _saveData.Init(time);

            if (_settings.SystemLogs)
                Debug.Log("[SaveManager]: Created clear save!");

            _isSaveLoaded = true;
        }

        private void Load(float time)
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

        public T GetSaveObject<T>(int hash) where T : ISaveObject, new()
        {
            if (!_isSaveLoaded)
            {
                if (_settings.SystemLogs)
                    Debug.LogError("[SaveManager]: Save system has not been initialized");

                return default;
            }

            return _saveData.GetSaveObject<T>(hash);
        }

        public T GetSaveObject<T>(string uniqueName) where T : ISaveObject, new() => GetSaveObject<T>(uniqueName.GetHashCode());

        public void Save()
        {
            if (!_isSaveRequired || _saveData == null)
                return;

            _saveData.Flush();
            _saveHandler.SaveData(_saveData, _settings.SaveFileName);

            if (_settings.SystemLogs)
                Debug.Log("[SaveManager]: Game is saved!");

            _isSaveRequired = false;
        }


        public void MarkAsSaveIsRequired() => _isSaveRequired = true;

        public void DeleteSaveFile()
        {
            _saveHandler.DeleteSave(_settings.SaveFileName);

            if (_saveData != null)
            {
                _saveData.ClearSaveData();
                _saveData = null;
            }

            _isSaveLoaded = false;
        }

        public void UpdateTime(float time)
        {
            if (_saveData != null)
                _saveData.Time = time;
        }

        private void StartAutoSaveTimer()
        {
            var interval = TimeSpan.FromSeconds(_settings.SaveDelay);
            _autoSaveTimer = new Timer(_ => Save(), null, interval, interval);
        }
    }
}
