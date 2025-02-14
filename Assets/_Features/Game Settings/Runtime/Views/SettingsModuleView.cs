using System;

[Serializable]
public abstract class SettingsModuleView
{
    public abstract void Initialize();
    //public abstract void SyncValues();
    public abstract void Dispose();
}
