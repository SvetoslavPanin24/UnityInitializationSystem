using System.Threading;
using UnityEngine;

namespace InitializationSystem
{
    [CreateAssetMenu(fileName = "DefaultSaveHandler", menuName = "InitializationSystem/Handlers/Default Save Handler")]
    public class DefaultSaveHandler : SaveHandler
    {
        [SerializeField] private Serializer.SerializeType _serializeType = Serializer.SerializeType.Binary;

        public override void Init() => IsInitialized = true;

        public override SaveData LoadSave(string fileName)
        {
            SaveData saveData = Serializer.DeserializeFromPDP<SaveData>(fileName, _serializeType, logIfFileNotExists: false);
            return saveData ?? new SaveData();
        }

        public override void SaveData(SaveData saveData, string fileName)
        {
            if (UseThreads())
            {
                Thread saveThread = new Thread(() => Serializer.SerializeToPDP(saveData, fileName, _serializeType));
                saveThread.Start();
            }
            else
                Serializer.SerializeToPDP(saveData, fileName, _serializeType);
        }

        public override void DeleteSave(string fileName) => Serializer.DeleteFileAtPDP(fileName);

        public override bool UseThreads() => true;
    }
}