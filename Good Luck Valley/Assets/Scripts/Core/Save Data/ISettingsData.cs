namespace HiveMind.SaveData
{
    public interface ISettingsData
    {
        void LoadData(SettingsData data);

        // Pass by reference so that the implementing script can modify the data
        void SaveData(SettingsData data);
    }
}
