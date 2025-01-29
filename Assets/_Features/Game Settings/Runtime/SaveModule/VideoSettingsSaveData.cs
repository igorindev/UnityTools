public class VideoSettingsSaveData : GameSettingsSaveModule<VideoSettingsData>
{
    public VideoSettingsSaveData(out VideoSettingsData currentVideoSettingsSaveData)
    {
        Load(out currentVideoSettingsSaveData);
    }
}
