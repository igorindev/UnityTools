using System;

namespace Modules
{
    public class Module { }

    [Serializable]
    public abstract class Module<TModule, TSaveModule, TModular> : Module
        where TSaveModule : SettingsSaveModule
        where TModular : Modular<TModular>, new()
    {
        protected readonly TSaveModule _settingsSaveModule;

        protected Module(SettingsSaveModule settingsSaveModule)
        {
            Modular<TModular>.AddModule<TModule>(this);
            _settingsSaveModule = settingsSaveModule as TSaveModule;
        }

        public abstract void Initialize();

        public abstract void Dispose();
    }
}
