using System.Collections.Generic;

public class VideoSettingsSaveData : GameSettingsSaveModule<VideoSettingsData>
{
    public VideoSettingsSaveData(out VideoSettingsData currentVideoSettingsSaveData)
    {
        Load(out currentVideoSettingsSaveData);
        currentVideoSettingsSaveData.settingsSaveModule ??= new Dictionary<System.Type, SettingsSaveModule>();
    }
}
