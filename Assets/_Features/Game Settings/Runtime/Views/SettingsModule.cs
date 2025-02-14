using System;

[Serializable]
public abstract class SettingsModule
{
    public abstract void Initialize();
    //public abstract void SyncValues();
    public abstract void Dispose();
}
