using System;

[Serializable]
public abstract class SettingsModule<TModule, TSaveModule> : SettingsModule where TSaveModule : SettingsSaveModule
{
    protected readonly TSaveModule _settingsSaveModule;

    protected SettingsModule(SettingsSaveModule settingsSaveModule)
    {
        VideoSettings.AddModule<TModule>(this);
        _settingsSaveModule = settingsSaveModule as TSaveModule;
    }

    public abstract void Initialize();

    //public abstract void SyncValues();
    public abstract void Dispose();
}

public class SettingsModule
{

}
