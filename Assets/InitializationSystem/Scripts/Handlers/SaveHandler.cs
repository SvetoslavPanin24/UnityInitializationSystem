namespace InitializationSystem
{
    public abstract class SaveHandler : Handler
    {
        public abstract SaveData LoadSave(string fileName);
        public abstract void SaveData(SaveData saveData, string fileName);
        public abstract void DeleteSave(string fileName);
        public virtual bool UseThreads() { return false; }
    }
}