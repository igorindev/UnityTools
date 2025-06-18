using System;
using UnityEngine;

namespace Modules
{
    public class Module : ScriptableObject
    {
        public Type Type => GetType();
        public virtual object GetSaveInstance() {  return null; }
    }

    public abstract class Module<TSaveModule> : Module where TSaveModule : SettingsSaveModule, new()
    {
        protected readonly TSaveModule _settingsSaveModule;

        public override object GetSaveInstance() { return new TSaveModule(); }
    }
}
