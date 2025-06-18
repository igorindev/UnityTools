using UnityEngine;

public class ModulesInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Initialize()
    {
        new VideoSettings();
    }
}
